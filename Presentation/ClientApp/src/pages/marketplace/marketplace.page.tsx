import {Fragment, useEffect, useState} from "react";
import {Dialog, Disclosure, Transition} from "@headlessui/react";
import {XMarkIcon} from "@heroicons/react/24/outline";
import {FunnelIcon, MinusIcon, PlusIcon} from "@heroicons/react/20/solid";
import {apiCaller} from "../../utils/api-caller.ts";
import {apiService} from "../../services/api.service.ts";
import {Category} from "../../services/category.service.ts";

// Dummy data
const items = [
  {
    id: 1,
    name: "Nomad Pouch",
    href: "#",
    price: "$50",
    availability: "White and Black",
    imageSrc: "https://tailwindui.com/img/ecommerce-images/category-page-07-product-01.jpg",
    imageAlt: "White fabric pouch with white zipper, black zipper pull, and black elastic loop.",
  },
  {
    id: 2,
    name: "Zip Tote Basket",
    href: "#",
    price: "$140",
    availability: "Washed Black",
    imageSrc: "https://tailwindui.com/img/ecommerce-images/category-page-07-product-02.jpg",
    imageAlt: "Front of tote bag with washed black canvas body, black straps, and tan leather handles and accents.",
  },
];

export const MarketplacePage = () => {
  const [mobileFiltersOpen, setMobileFiltersOpen] = useState(false);
  const [categories, setCategories] = useState<Category[]>([]);

  useEffect(() => {
    apiCaller(apiService.categories.getCategories(), {
      onSuccess: (response: Category[]) => {
        setCategories(response);
      },
      onError: (error: Error) => {
        console.error(error);
      }
    });
  }, []);

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
                                              className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
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
                                          className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
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
              <div className="grid grid-cols-1 gap-x-6 gap-y-10 sm:grid-cols-3 lg:col-span-3 lg:gap-x-8">
                {items.map((item) => (
                  <a key={item.id} href={item.href} className="group text-sm">
                    <div
                      className="aspect-h-1 aspect-w-1 w-full overflow-hidden rounded-lg bg-gray-100 group-hover:opacity-75">
                      <img
                        src={item.imageSrc}
                        alt={item.imageAlt}
                        className="h-full w-full object-cover object-center"
                      />
                    </div>
                    <h3 className="mt-4 font-medium text-gray-900">{item.name}</h3>
                    <p className="italic text-gray-500">{item.availability}</p>
                    <p className="mt-2 font-medium text-gray-900">{item.price}</p>
                  </a>
                ))}
              </div>
            </div>
          </section>
        </main>
      </div>
    </div>
  );
};
