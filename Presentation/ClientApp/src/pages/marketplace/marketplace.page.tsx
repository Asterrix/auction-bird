import {Fragment, useContext, useEffect, useRef, useState} from "react";
import {Dialog, Disclosure, Transition} from "@headlessui/react";
import {XMarkIcon} from "@heroicons/react/24/outline";
import {FunnelIcon, MinusIcon, PlusIcon} from "@heroicons/react/20/solid";
import {apiService} from "../../services/api.service.ts";
import {Page} from "../../utils/types/pagination/page.type.ts";
import {Pageable} from "../../utils/types/pagination/pageable.type.ts";
import {SearchContext} from "../../components/searchbar/search.provider.tsx";
import {Spinner} from "../../components/spinner/spinner.component.tsx";
import {ItemSummary} from "../../services/items/item.service.ts";
import {CategoriesContext} from "../../services/categories/category.provider.tsx";

export const MarketplacePage = () => {
  const [mobileFiltersOpen, setMobileFiltersOpen] = useState(false);


  const {categories} = useContext(CategoriesContext);
  const [items, setItems] = useState<Page<ItemSummary>>();

  const pageable: Pageable = {page: 1, size: 9};

  const {debouncedSearch} = useContext(SearchContext);
  const [categoriesFilter, setCategoriesFilter] = useState<string[]>([]);

  const [loadingMore, setLoadingMore] = useState(false);

  useEffect(() => {
    apiService.items.getItems(pageable)
      .then((response) => {
        setItems(response);
      })
      .catch((error) => {
        console.error(error);
      });
  }, []);

  const loadMoreItems = () => {
    setLoadingMore(true);

    pageable.page++;

    apiService.items.getItems(pageable, {search: debouncedSearch})
      .then((response) => {
        setItems({
          elements: items?.elements.concat(response.elements) || response.elements,
          isEmpty: response.isEmpty,
          totalPages: response.totalPages,
          totalElements: response.totalElements,
          isLastPage: response.isLastPage
        });
      })
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        setLoadingMore(false);
      });
  };

  // Filter
  useEffect(() => {
    pageable.page = 1;

    apiService.items.getItems(pageable, {search: debouncedSearch, categories: categoriesFilter})
      .then((response) => {
        setItems(response);
      })
      .catch((error) => {
        console.error(error);
      });
  }, [debouncedSearch, categoriesFilter]);

  const toggleCategoryFilter = (category: string) => {
    if (categoriesFilter.includes(category)) {
      setCategoriesFilter(categoriesFilter.filter((c) => c !== category));
    } else {
      setCategoriesFilter([...categoriesFilter, category]);
    }
  };

  // Infinite scroll
  const observerTarget = useRef(null);

  useEffect(() => {
    const observer = new IntersectionObserver((entries) => {
      if (entries[0].isIntersecting && items?.isLastPage === false) {
        loadMoreItems();
      }
    }, {threshold: 1});

    if (observerTarget.current) {
      observer.observe(observerTarget.current);
    }

    return () => {
      if (observerTarget.current) {
        observer.unobserve(observerTarget.current);
      }
    };
  }, [observerTarget, items?.isLastPage]);

  return (
    <div className="bg-white">
      <div>
        {/* Mobile filter dialog */}
        <Transition.Root show={mobileFiltersOpen} as={Fragment}>
          <Dialog as="div" className="relative z-40 lg:hidden" onClose={setMobileFiltersOpen}>
            <Transition.Child
              as={Fragment}
              enter="transition-opacity ease-linear duration-300"
              enterFrom="opacity-0"
              enterTo="opacity-100"
              leave="transition-opacity ease-linear duration-300"
              leaveFrom="opacity-100"
              leaveTo="opacity-0"
            >
              <div className="fixed inset-0 bg-black bg-opacity-25"/>
            </Transition.Child>

            <div className="fixed inset-0 z-40 flex">
              <Transition.Child
                as={Fragment}
                enter="transition ease-in-out duration-300 transform"
                enterFrom="translate-x-full"
                enterTo="translate-x-0"
                leave="transition ease-in-out duration-300 transform"
                leaveFrom="translate-x-0"
                leaveTo="translate-x-full"
              >
                <Dialog.Panel
                  className="relative ml-auto flex h-full w-full max-w-xs flex-col overflow-y-auto bg-white py-4 pb-12 shadow-xl">
                  <div className="flex items-center justify-between px-4">
                    <h2 className="text-lg font-medium text-gray-900">Filters</h2>
                    <button
                      type="button"
                      className="-mr-2 flex h-10 w-10 items-center justify-center rounded-md bg-white p-2 text-gray-400"
                      onClick={() => setMobileFiltersOpen(false)}
                    >
                      <span className="sr-only">Close menu</span>
                      <XMarkIcon className="h-6 w-6" aria-hidden="true"/>
                    </button>
                  </div>

                  {/* Filters Mobile */}
                  <form className="mt-4 border-t border-gray-200">

                    {/* Categories */}
                    <Disclosure as="div" key={"categories-mobile"} className="border-b px-4 py-6">
                      {({open}) => (
                        <>
                          <h3 className="-mx-2 -my-3 flow-root">
                            <Disclosure.Button
                              className="flex w-full items-center justify-between bg-white px-2 py-3 text-gray-400 hover:text-gray-500">
                              <span className="font-medium text-gray-900">Categories</span>
                              <span className="ml-6 flex items-center">
                                  {open ? (
                                    <MinusIcon className="h-5 w-5" aria-hidden="true"/>
                                  ) : (
                                    <PlusIcon className="h-5 w-5" aria-hidden="true"/>
                                  )}
                                </span>
                            </Disclosure.Button>
                          </h3>
                          <Disclosure.Panel className="pt-6">
                            {categories.map((category) => (
                              <Disclosure as="div" key={category.id} className="pl-2 py-6">
                                {({open}) => (
                                  <>
                                    <h3 className="-mx-2 -my-3 flow-root">
                                      <Disclosure.Button
                                        className="flex w-full items-center justify-between bg-white px-2 py-3 text-gray-400 hover:text-gray-500">
                                        <span className="font-medium text-gray-900">{category.name}</span>
                                        <span className="ml-6 flex items-center">
                                  {open ? (
                                    <MinusIcon className="h-5 w-5" aria-hidden="true"/>
                                  ) : (
                                    <PlusIcon className="h-5 w-5" aria-hidden="true"/>
                                  )}
                                </span>
                                      </Disclosure.Button>
                                    </h3>
                                    <Disclosure.Panel className="pt-6">
                                      <div className="space-y-6">
                                        {category.subcategories.map((category, index) => (
                                          <div key={category.name} className="flex items-center">
                                            <input
                                              id={`filter-mobile-${category.id}-${index}`}
                                              name={`${category.id}[]`}
                                              defaultValue={category.name}
                                              type="checkbox"
                                              defaultChecked={false}
                                              onClick={() => toggleCategoryFilter(category.name)}
                                              className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500 hover:cursor-pointer"
                                            />
                                            <label
                                              htmlFor={`filter-mobile-${category.id}-${index}`}
                                              className="ml-3 min-w-0 flex-1 text-gray-500"
                                            >
                                              {category.name}
                                            </label>
                                          </div>
                                        ))}
                                      </div>
                                    </Disclosure.Panel>
                                  </>
                                )}
                              </Disclosure>
                            ))}
                          </Disclosure.Panel>
                        </>
                      )}
                    </Disclosure>

                    {/* More filters... */}
                  </form>
                </Dialog.Panel>
              </Transition.Child>
            </div>
          </Dialog>
        </Transition.Root>

        <main className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="flex items-baseline justify-between border-b border-gray-200 pb-6 pt-24">
            <h1 className="text-4xl font-bold tracking-tight text-gray-900">Marketplace</h1>

            <div className="flex items-center">
              <button
                type="button"
                className="-m-2 ml-4 p-2 text-gray-400 hover:text-gray-500 sm:ml-6 lg:hidden"
                onClick={() => setMobileFiltersOpen(true)}
              >
                <span className="sr-only">Filters</span>
                <FunnelIcon className="h-5 w-5" aria-hidden="true"/>
              </button>
            </div>
          </div>

          <section aria-labelledby="items-heading" className="pb-24 pt-6">
            <h2 id="items-heading" className="sr-only">
              Items
            </h2>

            <div className="grid grid-cols-1 gap-x-8 gap-y-10 lg:grid-cols-4">
              {/* Filters */}
              <form className="hidden lg:block">

                {/* Categories */}
                <Disclosure as="div" key={"categories"} className="border-b py-6">
                  {({open}) => (
                    <>
                      <h3 className="-my-3 flow-root">
                        <Disclosure.Button
                          className="flex w-full items-center justify-between bg-white py-3 text-sm text-gray-400 hover:text-gray-500">
                          <span className="font-medium text-gray-900">Categories</span>
                          <span className="ml-6 flex items-center">
                              {open ? (
                                <MinusIcon className="h-5 w-5" aria-hidden="true"/>
                              ) : (
                                <PlusIcon className="h-5 w-5" aria-hidden="true"/>
                              )}
                            </span>
                        </Disclosure.Button>
                      </h3>
                      <Disclosure.Panel className="pl-2 mt-6">
                        {categories.map((category) => (
                          <Disclosure as="div" key={category.id} className="py-4">
                            {({open}) => (
                              <>
                                <h3 className="-my-3 flow-root">
                                  <Disclosure.Button
                                    className="flex w-full items-center justify-between bg-white py-3 text-sm text-gray-400 hover:text-gray-500">
                                    <span className="font-medium text-gray-900">{category.name}</span>
                                    <span className="ml-6 flex items-center">
                                      {open ? (
                                        <MinusIcon className="h-5 w-5" aria-hidden="true"/>
                                      ) : (
                                        <PlusIcon className="h-5 w-5" aria-hidden="true"/>
                                      )}
                                    </span>
                                  </Disclosure.Button>
                                </h3>
                                <Disclosure.Panel className="pt-6">
                                  <div className="space-y-4">
                                    {category.subcategories.map((option, optionIdx) => (
                                      <div key={option.id} className="flex items-center">
                                        <input
                                          id={`filter-${category.id}-${optionIdx}`}
                                          name={`${category.id}[]`}
                                          defaultValue={option.name}
                                          type="checkbox"
                                          defaultChecked={false}
                                          onClick={() => toggleCategoryFilter(option.name)}
                                          className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500 hover:cursor-pointer"
                                        />
                                        <label
                                          htmlFor={`filter-${category.id}-${optionIdx}`}
                                          className="ml-3 text-sm text-gray-600"
                                        >
                                          {option.name}
                                        </label>
                                      </div>
                                    ))}
                                  </div>
                                </Disclosure.Panel>
                              </>
                            )}
                          </Disclosure>
                        ))}
                      </Disclosure.Panel>
                    </>
                  )}
                </Disclosure>

                {/* More filters... */}
              </form>

              {/* Item grid */}
              <div className="grid grid-cols-1 gap-x-6 gap-y-10 sm:grid-cols-3 lg:col-span-3 lg:gap-x-8 relative">
                {items?.elements.map((item) => (
                  <a key={item.id} href={`item/${item.id}`} className="group text-sm">
                    <div
                      className="aspect-h-1 aspect-w-1 w-full overflow-hidden rounded-lg bg-gray-100 group-hover:opacity-75">
                      <img
                        src={item.mainImage.imageUrl}
                        alt={item.name}
                        className="h-full w-full object-cover object-center"
                      />
                    </div>
                    <h3 className="mt-4 font-medium text-gray-900">{item.name}</h3>
                    <p className="mt-2 font-medium text-indigo-500">${item.initialPrice}</p>
                  </a>
                ))}

                {loadingMore && <div className="absolute my-10 left-1/2 right-1/2 top-full"><Spinner/></div>}
                {items?.isEmpty && <p className="text-center text-gray-500">No items were found.</p>}
                <div ref={observerTarget}></div>
              </div>

            </div>
          </section>
        </main>
      </div>
    </div>
  );
};
