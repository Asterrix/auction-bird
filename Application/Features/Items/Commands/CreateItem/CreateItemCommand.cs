using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Application.Features.Authentication.Queries;
using Application.Features.Categories.Queries.FindCategory;
using Application.Features.Items.Mapper;
using Domain.Categories;
using Domain.Exceptions;
using Domain.Items;
using Firebase.Storage;
using FluentValidation;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using FirebaseStorageException = Domain.Exceptions.FirebaseStorageException;

namespace Application.Features.Items.Commands.CreateItem;

public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(p => p.Item.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long.")
            .MaximumLength(32)
            .WithMessage("Name must not exceed 32 characters.");

        RuleFor(p => p.Item.Category)
            .NotEmpty()
            .WithMessage("Category is required.");

        RuleFor(p => p.Item.Subcategory)
            .NotEmpty()
            .WithMessage("Subcategory is required.");

        RuleFor(p => p.Item.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MinimumLength(20)
            .WithMessage("Description must be at least 20 characters long.")
            .MaximumLength(700)
            .WithMessage("Description must not exceed 700 characters.");

        RuleFor(p => p.Item.InitialPrice)
            .NotEmpty()
            .WithMessage("Initial price is required.")
            .GreaterThan(1.00m)
            .WithMessage("Initial price must be at least $1.00.")
            .LessThan(9_999_999.99m)
            .WithMessage("Initial price must be less than $9,999,999.99.");

        RuleFor(p => p.Item.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.")
            .GreaterThan(DateTime.UtcNow + TimeSpan.FromDays(1))
            .WithMessage("Start time must be at least 1 day in the future.");

        RuleFor(p => p.Item.EndTime)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(p => p.Item.StartTime)
            .WithMessage("End time must be after start time.")
            .LessThan(p => p.Item.StartTime + TimeSpan.FromDays(31))
            .WithMessage("End time must be within 31 days of start time.");

        RuleFor(p => p.Item.Images)
            .NotEmpty()
            .WithMessage("At least 3 images are required.")
            .Must(images => images.All(image => image.ContentType.StartsWith("image/")))
            .WithMessage("Only image files are allowed.")
            .Must(images => images.Count > 2)
            .WithMessage("At least 3 images are required.")
            .Must(images => images.Count < 33)
            .WithMessage("No more than 32 images are allowed.");
    }
}

public record CreateItemCommand(CreateItemDto Item) : IRequest<bool>;

public sealed class CreateItemCommandHandler(
    ISender sender,
    IValidator<CreateItemCommand> validator,
    IItemRepository itemRepository,
    FirebaseStorage firebaseStorage)
    : IRequestHandler<CreateItemCommand, bool>
{
    public async Task<bool> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        Option<UserType> userOption = await sender.Send(new FindUserQuery(request.Item.OwnerId), cancellationToken);
        UserType user = userOption.IfNone(() => throw new AmazonServiceException("Owner not found."));

        Option<Category> categoryOption =
            await sender.Send(new FindCategoryQuery(request.Item.Category), cancellationToken);
        Category category = categoryOption.IfNone(() => throw new NotFoundException("Category not found."));

        Option<Category> subcategoryOption =
            await sender.Send(new FindCategoryQuery(request.Item.Subcategory), cancellationToken);
        Category subcategory = subcategoryOption.IfNone(() => throw new NotFoundException("Subcategory not found."));

        if (subcategory.Parent?.Name != category.Name)
            throw new ValidationException("Subcategory must belong to the specified category.");

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        Item item = new()
        {
            Name = request.Item.Name.Trim(),
            CategoryId = subcategory.Id,
            Description = request.Item.Description,
            InitialPrice = request.Item.InitialPrice,
            StartTime = request.Item.StartTime.ToUniversalTime(),
            EndTime = request.Item.EndTime.ToUniversalTime(),
            OwnerId = user.Username,
            IsActive = true,
            Images = []
        };

        List<ItemImage> itemImages = await UploadImagesToFirebaseStorage(request.Item.Images, item.Id);
        item.Images.AddRange(itemImages);

        bool result = await itemRepository.CreateAsync(item, cancellationToken);

        return result;
    }

    private async Task<List<ItemImage>> UploadImagesToFirebaseStorage(List<IFormFile> images, Guid itemId)
    {
        List<ItemImage> itemImages = [];

        foreach (IFormFile image in images)
        {
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

            try
            {
                string imageUrl = await firebaseStorage
                    .Child("items")
                    .Child(fileName)
                    .PutAsync(image.OpenReadStream());
                
                itemImages.Add(new ItemImage { ImageUrl = imageUrl, ItemId = itemId });
            }
            catch (Exception)
            {
                throw new FirebaseStorageException("An error occurred while uploading images.");
            }
        }

        return itemImages;
    }
}