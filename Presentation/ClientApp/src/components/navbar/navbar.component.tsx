import {Fragment, useContext, useEffect, useState} from "react";
import {MobileSidebar} from "./mobile-sidebar.component.tsx";
import {Menu, Popover, Transition} from "@headlessui/react";
import {classNames} from "../../utils/tailwind/class-names.utils.ts";
import {SearchBar} from "../searchbar/searchbar.component.tsx";
import {Bars3Icon} from "@heroicons/react/24/outline";
import {CategoriesContext} from "../../services/categories/category.provider.tsx";
import {Category} from "../../services/categories/category.service.ts";
import {userContext} from "../../services/auth/user.provider.tsx";
import {useNavigate, useNavigation} from "react-router-dom";

export interface NavbarNavigation {
  featured: {
    name: string;
    href: string;
    imageSrc: string;
    imageAlt: string;
  }[];
  categories: Category[];
  pages: { name: string; href: string }[];
}

const navbarNavigation: NavbarNavigation = {
  featured: [
    {
      name: "New Arrivals",
      href: "#",
      imageSrc: "https://tailwindui.com/img/ecommerce-images/mega-menu-category-01.jpg",
      imageAlt: "Models sitting back to back, wearing Basic Tee in black and bone.",
    },
    {
      name: "Basic Tees",
      href: "#",
      imageSrc: "https://tailwindui.com/img/ecommerce-images/mega-menu-category-02.jpg",
      imageAlt: "Close up of Basic Tee fall bundle with off-white, ochre, olive, and black tees.",
    },
  ],
  categories: [],
  pages: [
    {name: "Marketplace", href: "/marketplace"},
  ],
};

export const NavbarComponent = () => {
  const [open, setOpen] = useState(false);
  const {user, signOut} = useContext(userContext);
  const {categories} = useContext(CategoriesContext);
  const [navigation, setNavigation] = useState<NavbarNavigation>({
    categories: [],
    pages: navbarNavigation.pages,
    featured: navbarNavigation.featured
  });

  useEffect(() => {
    if (categories) {
      setNavigation({
        ...navigation,
        categories: categories
      });
    }
  }, [categories]);

  return (
    <>
      <MobileSidebar open={open} setOpen={setOpen} navigation={navigation}/>

      <nav aria-label="Top" className="fixed w-full z-20 bg-white bg-opacity-90 backdrop-blur-xl backdrop-filter">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="flex h-16 items-center">

            {/* Logo */}
            <div className="ml-4 flex lg:ml-0">
              <span className="sr-only">AuctionBird</span>
              <img
                className="h-8 w-auto"
                src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600"
                alt="AuctionBird logo"
              />
            </div>

            {/* Flyout menus */}
            <Popover.Group className="hidden lg:ml-8 lg:block lg:self-stretch">
              <div className="flex h-full space-x-8">

                {/* Home Page */}
                <a
                  key={"home"}
                  href={"/"}
                  className="flex items-center text-sm font-medium text-gray-700 hover:text-gray-800"
                >
                  Home
                </a>

                <Popover className="flex">
                  {({open}) => (
                    <>
                      <div className="relative flex">
                        <Popover.Button
                          className={classNames(
                            open
                              ? "border-indigo-600 text-indigo-600"
                              : "border-transparent text-gray-700 hover:text-gray-800",
                            "relative z-10 -mb-px flex items-center border-b-2 pt-px text-sm font-medium transition-colors duration-200 ease-out outline-none"
                          )}
                        >
                          Categories
                        </Popover.Button>
                      </div>

                      <Transition
                        as={Fragment}
                        enter="transition ease-out duration-200"
                        enterFrom="opacity-0"
                        enterTo="opacity-100"
                        leave="transition ease-in duration-150"
                        leaveFrom="opacity-100"
                        leaveTo="opacity-0"
                      >
                        <Popover.Panel className="absolute inset-x-0 top-full bg-white text-sm text-gray-500">
                          {/* Presentational element used to render the bottom shadow, if we put the shadow on the actual panel it pokes out the top, so we use this shorter element to hide the top of the shadow */}
                          <div className="absolute inset-0 top-1 bg-white shadow" aria-hidden="true"/>
                          {/* Fake border when menu is open */}
                          <div className="absolute inset-0 top-0 mx-auto h-px max-w-7xl px-8" aria-hidden="true">
                            <div
                              className={classNames(
                                open ? "bg-gray-200" : "bg-transparent",
                                "h-px w-full transition-colors duration-200 ease-out"
                              )}
                            />
                          </div>

                          <div className="relative">
                            <div className="mx-auto max-w-7xl px-8">
                              <div className="grid grid-cols-2 gap-x-8 gap-y-10 py-16">

                                {/* Featured Selection */}
                                <div className="col-start-2 grid grid-cols-2 gap-x-8">
                                  {navigation.featured.map((item) => (
                                    <div key={item.name} className="group relative text-base sm:text-sm">
                                      <div
                                        className="aspect-h-1 aspect-w-1 overflow-hidden rounded-lg bg-gray-100 group-hover:opacity-75">
                                        <img
                                          src={item.imageSrc}
                                          alt={item.imageAlt}
                                          className="object-cover object-center"
                                        />
                                      </div>
                                      <a href={item.href} className="mt-6 block font-medium text-gray-900">
                                        <span className="absolute inset-0 z-10" aria-hidden="true"/>
                                        {item.name}
                                      </a>
                                      <p aria-hidden="true" className="mt-1">
                                        Shop now
                                      </p>
                                    </div>
                                  ))}
                                </div>

                                {/* Categories */}
                                <div className="row-start-1 grid grid-cols-3 gap-x-8 gap-y-10 text-sm">
                                  {navigation.categories.map((section) => (
                                    <div key={section.name}>
                                      <p id={`${section.name}-heading`} className="font-medium text-gray-900">
                                        {section.name}
                                      </p>
                                      <ul
                                        role="list"
                                        aria-labelledby={`${section.name}-heading`}
                                        className="mt-6 space-y-6 sm:mt-4 sm:space-y-4"
                                      >
                                        {section.subcategories.map((item) => (
                                          <li key={item.name} className="flex">
                                            <a href="#" className="hover:text-gray-800">
                                              {item.name}
                                            </a>
                                          </li>
                                        ))}
                                      </ul>
                                    </div>
                                  ))}
                                </div>

                              </div>
                            </div>
                          </div>
                        </Popover.Panel>
                      </Transition>
                    </>
                  )}
                </Popover>

                {/* Pages */}
                {navigation.pages.map((page) => (
                  <a
                    key={page.name}
                    href={page.href}
                    className="flex items-center text-sm font-medium text-gray-700 hover:text-gray-800"
                  >
                    {page.name}
                  </a>
                ))}
              </div>
            </Popover.Group>

            {/* Search */}
            <SearchBar/>

            {/* Toggle Mobile Sidebar Menu */}
            <button
              type="button"
              className="rounded-md bg-white p-2 text-gray-400 lg:hidden"
              onClick={() => setOpen(true)}>
              <span className="sr-only">Open menu</span>
              <Bars3Icon className="h-6 w-6" aria-hidden="true"/>
            </button>

            {/* Action Menu */}
            <div className="ml-auto flex items-center">
              <div className="hidden lg:flex lg:flex-1 lg:items-center lg:justify-end lg:space-x-6">
                {user?.present ? (
                  <Menu as="div" className="relative ml-4 flex-shrink-0">
                    <div>
                      <Menu.Button
                        className="flex rounded-full bg-white text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2">
                        <span className="sr-only">Open user menu</span>
                        <img
                          className="h-8 w-8 rounded-full"
                          src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80"
                          alt=""
                        />
                      </Menu.Button>
                    </div>
                    <Transition
                      as={Fragment}
                      enter="transition ease-out duration-100"
                      enterFrom="transform opacity-0 scale-95"
                      enterTo="transform opacity-100 scale-100"
                      leave="transition ease-in duration-75"
                      leaveFrom="transform opacity-100 scale-100"
                      leaveTo="transform opacity-0 scale-95"
                    >
                      <Menu.Items
                        className="absolute right-0 z-10 mt-2 w-48 origin-top-right rounded-md bg-white py-1 shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none">
                        <Menu.Item>
                          {({active}) => (
                            <button
                              onClick={() => window.location.href = "/profile/active"}
                              className={classNames(active ? "bg-gray-100" : "", "flex w-full px-4 py-2 text-sm text-gray-700")}
                            >
                              Profile
                            </button>
                          )}
                        </Menu.Item>
                        <Menu.Item>
                          {({active}) => (
                            <button
                              onClick={signOut}
                              className={classNames(active ? "bg-gray-100" : "", "flex w-full px-4 py-2 text-sm text-gray-700")}
                            >
                              Sign out
                            </button>
                          )}
                        </Menu.Item>
                      </Menu.Items>
                    </Transition>
                  </Menu>
                ) : (
                  <>
                    <a href="/signin" className="text-sm font-medium text-gray-700 hover:text-gray-800">
                      Sign in
                    </a>
                    <span className="h-6 w-px bg-gray-200" aria-hidden="true"/>
                    <a href="/signup" className="text-sm font-medium text-gray-700 hover:text-gray-800">
                      Create account
                    </a>
                  </>
                )}
              </div>
            </div>
          </div>
        </div>
      </nav>
    </>
  );
};