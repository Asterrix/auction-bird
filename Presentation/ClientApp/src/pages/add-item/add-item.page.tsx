import {PhotoIcon} from "@heroicons/react/24/solid";
import {ExclamationCircleIcon} from "@heroicons/react/20/solid";
import {ChangeEvent, useContext, useState} from "react";
import {CategoriesContext} from "../../services/categories/category.provider.tsx";
import {FieldValues, useForm} from "react-hook-form";
import {classNames} from "../../utils/tailwind/class-names.utils.ts";
import {XMarkIcon} from "@heroicons/react/24/outline";
import {DateTimePicker} from "../profile/date-time-picker.component.tsx";
import {Modal} from "../../components/modal/modal.component.tsx";
import {apiService} from "../../services/api.service.ts";
import {CreateItem} from "../../services/items/item.service.ts";
import {useNavigate} from "react-router-dom";
import {Spinner} from "../../components/spinner/spinner.component.tsx";

export const AddItemPage = () => {
  const {
    register,
    getValues,
    handleSubmit,
    watch,
    formState: {errors},
  } = useForm();
  const navigate = useNavigate();
  const [processingRequest, setProcessingRequest] = useState(false);

  // Categories
  const {categories} = useContext(CategoriesContext);
  const selectedCategory = watch("category");

  // Images
  const [selectedImages, setSelectedImages] = useState([]);
  const [selectedImageFiles, setSelectedImageFiles] = useState<File[]>([]);

  const handleImagesChange = (e: ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const allowedFormats = ["image/png", "image/jpeg", "image/jpg"];
      if (Array.from(e.target.files).some((file) => !allowedFormats.includes(file.type))) {
        return;
      }

      const maxSize = 10 * 1024 * 1024; // 10MB
      if (Array.from(e.target.files).some((file) => file.size > maxSize)) {
        return;
      }

      const files = Array.from(e.target.files).map((file) => URL.createObjectURL(file));
      setSelectedImages((prevImages) => [...prevImages, ...files]);
      setSelectedImageFiles((prevFiles) => [...prevFiles, ...Array.from(e.target.files)]);
    }
  };

  const removeImage = (index: number) => {
    setSelectedImages((prevImages) => prevImages.filter((_, i) => i !== index));
  };

  // Description
  const [descriptionWordCount, setDescriptionWordCount] = useState(0);
  const updateDescriptionWordCount = () => {
    setDescriptionWordCount(getValues("description").length);
  };

  // DateTimePicker
  const [dateTimePickerOpen, setOpenDateTimePicker] = useState({
    startDateTime: false,
    endDateTime: false,
  });
  const [startDateTime, setStartDateTime] = useState<Date | null>();
  const [endDateTime, setEndDateTime] = useState<Date | null>();

  const submitStartDateTime = (date: Date) => {
    if (date !== undefined) {
      setStartDateTime(date);
    }
    setOpenDateTimePicker((prev) => ({...prev, startDateTime: false}));
  };

  const submitEndDateTime = (date: Date) => {
    if (date !== undefined) {
      setEndDateTime(date);
    }
    setOpenDateTimePicker((prev) => ({...prev, endDateTime: false}));
  };

  const openDateTimePicker = (type: "startDateTime" | "endDateTime") => {
    setOpenDateTimePicker((prev) => ({...prev, [type]: true}));
  };

  const closeDateTimePicker = () => {
    setOpenDateTimePicker((prev) => ({...prev, startDateTime: false, endDateTime: false}));
  };

  const submitForm = (data: FieldValues) => {
    setProcessingRequest(true);

    const item: CreateItem = {
      name: data.name,
      category: data.category,
      subcategory: data.subcategory,
      description: data.description,
      images: selectedImageFiles,
      initialPrice: data.initialPrice,
      startTime: startDateTime!,
      endTime: endDateTime!
    };

    apiService.items.createItem(item)
      .then(() => {
        navigate("/profile/active");
      })
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        setProcessingRequest(false);
      });
  };

  return (
    <>
      {processingRequest && (
        <Modal children={
          <Spinner/>
        } disableClose={true}/>
      )}

      <form className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 pt-24"
            onSubmit={handleSubmit(data => submitForm(data))}
            encType="multipart/form-data">
        <div className="space-y-12">
          <div className="border-b border-gray-900/10 pb-12">
            <h2 className="text-base font-semibold leading-7 text-gray-900">Add new item</h2>
            <p className="mt-1 text-sm leading-6 text-gray-600">
              Get started by filling in the information below to create your new item.
            </p>

            <div className="mt-10 grid grid-cols-1 gap-x-6 gap-y-8 sm:grid-cols-6">
              <div className="col-span-full">
                <label htmlFor="name" className="block text-sm font-medium leading-6 text-gray-900">
                  Name
                </label>
                <div className="relative mt-2 rounded-md shadow-sm">
                  <input
                    type="text"
                    id="name"
                    {...register("name", {
                      required: {value: true, message: "Name is required."},
                      minLength: {value: 3, message: "Name must be at least 3 characters long."},
                      maxLength: {value: 80, message: "Name cannot exceed 80 characters."}
                    })}
                    className={classNames(errors.name ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                    )}
                    placeholder="Playstation 5"
                  />
                  {errors.name && (
                    <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                      <ExclamationCircleIcon className="h-5 w-5 text-red-500" aria-hidden="true"/>
                    </div>
                  )}
                </div>
                {errors.name && (
                  <p className="mt-2 text-sm text-red-600" id="name-error">
                    {errors.name.message}
                  </p>
                )}
              </div>

              <div className="grid grid-cols-2 gap-6 col-span-full">
                <div>
                  <label htmlFor="category" className="block text-sm font-medium leading-6 text-gray-900">
                    Category
                  </label>
                  <select
                    id="category"
                    {...register("category", {
                      required: {value: true, message: "Category is required."}
                    })}
                    className={classNames(errors.category ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full mt-2 rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6 cursor-pointer"
                    )}
                  >
                    {!selectedCategory && <option value="" className="text-gray-400">Select a category</option>}
                    {categories.map((category) => (
                      <option key={category.id}>{category.name}</option>
                    ))}
                  </select>
                  {errors.category && (
                    <p className="mt-2 text-sm text-red-600" id="name-error">
                      {errors.category.message}
                    </p>
                  )}
                </div>

                <div>
                  <label htmlFor="subcategory" className="block text-sm font-medium leading-6 text-gray-900">
                    Subcategory
                  </label>
                  <select
                    id="subcategory"
                    {...register("subcategory", {
                      required: {value: true, message: "Subcategory is required."}
                    })}
                    disabled={!selectedCategory}
                    className={classNames(errors.subcategory ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full mt-2 rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6 cursor-pointer"
                    )}
                  >
                    {categories.find((category) => category.name === selectedCategory)?.subcategories.map((subcategory) => (
                      <option key={subcategory.id}>{subcategory.name}</option>
                    ))}
                  </select>
                  {errors.subcategory && (
                    <p className="mt-2 text-sm text-red-600" id="name-error">
                      {errors.subcategory.message}
                    </p>
                  )}
                </div>
              </div>

              <div className="col-span-full">
                <label htmlFor="description" className="block text-sm font-medium leading-6 text-gray-900">
                  Description
                </label>
                <div className="mt-2">
                <textarea
                  id="description"
                  {...register("description", {
                    required: {value: true, message: "Description is required."},
                    minLength: {value: 20, message: "Description must be at least 20 characters long."},
                    maxLength: {value: 700, message: "Description cannot exceed 700 characters."},
                    onChange: updateDescriptionWordCount
                  })}
                  rows={5}
                  className={classNames(errors.description ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                    "block w-full mt-2 rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                  )}
                  defaultValue={""}
                />
                </div>
                <div className="flex justify-between">
                  {errors.description ? (
                    <p className="mt-2 text-sm text-red-600" id="name-error">
                      {errors.description.message}
                    </p>
                  ) : <div></div>}
                  <p
                    className={classNames(errors.description ? "text-red-600" : "text-gray-600", "mt-2 text-sm leading-6")}>{descriptionWordCount}/700</p>
                </div>
              </div>

              <div className="col-span-full">
                <label htmlFor="images" className="block text-sm font-medium leading-6 text-gray-900">
                  Images of the item
                </label>
                <div
                  className={classNames(errors.images ? "border-red-500" : "border-gray-900/25", "mt-2 flex justify-center rounded-lg border border-dashed border-gray-900/25 px-6 py-10")}>

                  {selectedImages.length === 0 && (
                    <div className="text-center">
                      <PhotoIcon className="mx-auto h-12 w-12 text-gray-300" aria-hidden="true"/>
                      <div className="mt-4 flex text-sm leading-6 text-gray-600">
                        <label
                          htmlFor="file-upload"
                          className="relative cursor-pointer w-full rounded-md bg-white font-semibold text-indigo-600 ring-gray-300 focus:ring-indigo-600 focus-within:outline-none focus-within:ring-2 focus-within:ring-indigo-600 focus-within:ring-offset-2 hover:text-indigo-500"
                        >
                          <span>Upload a file</span>
                          <input
                            id="file-upload"
                            {...register("images", {
                              validate: () => {
                                if (selectedImages.length === 0) {
                                  return "Images are required.";
                                }
                                return true;
                              },
                            })}
                            onChange={handleImagesChange}
                            type="file"
                            multiple={true}
                            className="sr-only"/>
                        </label>
                      </div>
                      <p className="text-xs leading-5 text-gray-600">PNG, JPG, JPEG up to 10MB</p>
                    </div>
                  )}

                  {selectedImages.length > 0 && (
                    <div className="grid grid-cols-12 gap-3 w-full h-full relative">
                      {selectedImages.map((image, index) => (
                        <div className="flex items-center justify-center relative" key={index}>
                          <img key={index} src={image} alt="item" className="h-20 w-20 object-cover rounded-md z-40"/>
                          <div className="absolute top-1 right-1 flex">
                            <button
                              type="button"
                              className="rounded-md text-red-500 z-50 hover:text-red-700 focus:outline-none focus:ring-2 focus:ring-red-500"
                              onClick={() => removeImage(index)}
                            >
                              <span className="sr-only">Remove image</span>
                              <XMarkIcon className="h-6 w-6" aria-hidden="true"/>
                            </button>
                          </div>
                        </div>
                      ))}
                      <label
                        htmlFor="file-upload"
                        className="absolute w-full h-full cursor-pointer font-semibold text-indigo-600 focus-within:outline-none focus-within:ring-2 focus-within:ring-indigo-600 focus-within:ring-offset-2 hover:text-indigo-500"
                      >
                        <span className="sr-only">Upload a file</span>
                        <input
                          id="file-upload"
                          {...register("images", {
                            validate: () => {
                              if (selectedImages.length > 32) {
                                return "You can upload up to 32 images.";
                              }
                              return true;
                            },
                          })}
                          onChange={handleImagesChange}
                          type="file"
                          multiple={true}
                          className="sr-only"/>
                      </label>
                    </div>
                  )}
                </div>

                {errors.images && (
                  <p className="mt-2 text-sm text-red-600" id="name-error">
                    {errors.images.message}
                  </p>
                )}
              </div>

              <div className="col-span-2">
                <label htmlFor="initial-price" className="block text-sm font-medium leading-6 text-gray-900">
                  Initial Price
                </label>
                <div className="relative mt-2 rounded-md shadow-sm">
                  <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                    <span className="text-gray-500 sm:text-sm">$</span>
                  </div>
                  <input
                    type="text"
                    id="initial-price"
                    {...register("initialPrice", {
                      required: {value: true, message: "Initial price is required."},
                      pattern: {value: /^\d+(\.\d{1,2})?$/, message: "Invalid price format."},
                      min: {value: 1.00, message: "Initial price must be at least $1.00."},
                      max: {value: 9999999.99, message: "Initial price cannot exceed $9,999,999.99."}
                    })}
                    className={classNames(errors.initialPrice ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full rounded-md border-0 py-1.5 pl-7 pr-12 text-gray-900 ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset  sm:text-sm sm:leading-6")}
                    placeholder="100.00"
                    aria-describedby="price-currency"
                  />
                  <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                <span className="text-gray-500 sm:text-sm" id="price-currency">
                  USD
                </span>
                  </div>
                </div>
                {errors.initialPrice && (
                  <p className="mt-2 text-sm text-red-600" id="name-error">
                    {errors.initialPrice.message}
                  </p>
                )}
              </div>

              <div className="col-span-2">
                <label htmlFor="start-date-time" className="block text-sm font-medium leading-6 text-gray-900">
                  Start date and time
                </label>
                <div className="relative mt-2 rounded-md shadow-sm">
                  <input
                    onClick={() => openDateTimePicker("startDateTime")}
                    readOnly
                    type="text"
                    id="start-date-time"
                    className={classNames(errors.startDateTime ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                    )}
                    {...register("startDateTime", {
                      required: {value: true, message: "Start date and time is required."}
                    })}
                    placeholder="Select start date and time"
                    value={startDateTime?.toLocaleString()}
                  />
                  {dateTimePickerOpen.startDateTime && <Modal children={
                    <DateTimePicker onAccept={submitStartDateTime} onClose={closeDateTimePicker}/>
                  }/>}
                </div>
                {errors.startDateTime && (
                  <p className="mt-2 text-sm text-red-600" id="name-error">
                    {errors.startDateTime.message}
                  </p>
                )}
              </div>

              <div className="col-span-2">
                <label htmlFor="end-date-time" className="block text-sm font-medium leading-6 text-gray-900">
                  End date and time
                </label>
                <div className="relative mt-2 rounded-md shadow-sm">
                  <input
                    onClick={() => openDateTimePicker("endDateTime")}
                    readOnly
                    type="text"
                    id="end-date-time"
                    className={classNames(errors.endDateTime ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                    )}
                    {...register("endDateTime", {
                      required: {value: true, message: "End date and time is required."},
                      validate: (value) => {
                        if (startDateTime && new Date(value) < startDateTime) {
                          return "End date and time must come after start date and time.";
                        }
                        return true;
                      }
                    })}
                    placeholder="Select end date and time"
                    value={endDateTime?.toLocaleString()}
                  />
                  {dateTimePickerOpen.endDateTime && <Modal children={
                    <DateTimePicker onAccept={submitEndDateTime} onClose={closeDateTimePicker}/>
                  }/>}
                </div>
                {errors.endDateTime && (
                  <p className="mt-2 text-sm text-red-600" id="name-error">
                    {errors.endDateTime.message}
                  </p>
                )}
              </div>
            </div>
          </div>
        </div>

        <div className="mt-6 flex items-center justify-end gap-x-6">
          <button type="button" className="text-sm font-semibold leading-6 text-gray-900">
            Cancel
          </button>
          <button
            type="submit"
            className="rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
          >
            Save
          </button>
        </div>
      </form>
    </>
  );
};