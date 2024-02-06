import React, {Fragment} from "react";
import {Dialog, Tab, Transition} from "@headlessui/react";
import {XMarkIcon} from "@heroicons/react/24/outline";
import {NavbarNavigation} from "./navbar.component.tsx";

interface MobileSidebarProps {
  open: boolean;
  setOpen: (v: boolean) => void;
  navigation: NavbarNavigation;
}

export const MobileSidebar: React.FC<MobileSidebarProps> = (
  {
    open,
    setOpen,
    navigation
  }) => {
  return (
    <Transition.Root show={open} as={Fragment}>
      <Dialog as="div" className="relative z-40 lg:hidden" onClose={setOpen}>

        {/* Background Overlay */}
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

        {/* Main Content */}
        <div className="fixed inset-0 z-40 flex">
          <Transition.Child
            as={Fragment}
            enter="transition ease-in-out duration-300 transform"
            enterFrom="-translate-x-full"
            enterTo="translate-x-0"
            leave="transition ease-in-out duration-300 transform"
            leaveFrom="translate-x-0"
            leaveTo="-translate-x-full"
          >
            <Dialog.Panel className="relative flex w-full max-w-xs flex-col overflow-y-auto bg-white pb-12 shadow-xl">

              {/* Close Button */}
              <div className="flex px-4 pt-5 pb-2">
                <button
                  type="button"
                  className="-m-2 inline-flex items-center justify-center rounded-md p-2 text-gray-400"
                  onClick={() => setOpen(false)}
                >
                  <span className="sr-only">Close menu</span>
                  <XMarkIcon className="h-6 w-6" aria-hidden="true"/>
                </button>
              </div>

              {/* Featured Links */}
              <Tab.Group as="div" className="mt-2">
                <Tab.Panels as={Fragment}>
                  <Tab.Panel className="px-4 pt-10 pb-8 space-y-10">

                    {/* Featured Section */}
                    <div className="grid grid-cols-2 gap-x-4">
                      {navigation.featured.map((item) => (
                        <div key={item.name} className="relative text-sm group">
                          {/* Featured Item */}
                          <div
                            className="overflow-hidden rounded-lg bg-gray-100 aspect-h-1 aspect-w-1 group-hover:opacity-75">
                            <img src={item.imageSrc} alt={item.imageAlt} className="object-cover object-center"/>
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
                    {navigation.categories.map((section) => (
                      <div key={section.name}>
                        <p id={`${section.id}-heading-mobile`} className="font-medium text-gray-900">
                          {section.name}
                        </p>
                        <ul
                          role="list"
                          aria-labelledby={`${section.id}-heading-mobile`}
                          className="mt-6 flex flex-col space-y-6"
                        >
                          {section.items.map((item) => (
                            <li key={item.name} className="flow-root">
                              <a href={item.href} className="-m-2 block p-2 text-gray-500">
                                {item.name}
                              </a>
                            </li>
                          ))}
                        </ul>
                      </div>
                    ))}
                  </Tab.Panel>
                </Tab.Panels>
              </Tab.Group>

              {/* Pages */}
              <div className="border-t border-gray-200 px-4 py-6 space-y-6">

                {/* Home Page */}
                <div key="home" className="flow-root">
                  <a href="/" className="-m-2 block p-2 font-medium text-gray-900">
                    Home
                  </a>
                </div>

                {/* Other Pages */}
                {navigation.pages.map((page) => (
                  <div key={page.name} className="flow-root">
                    <a href={page.href} className="-m-2 block p-2 font-medium text-gray-900">
                      {page.name}
                    </a>
                  </div>
                ))}
              </div>

              {/* Action Menu */}
              <div className="border-t border-gray-200 px-4 py-6 space-y-6">

                {/* Sign In */}
                <div className="flow-root">
                  <a href="#" className="-m-2 block p-2 font-medium text-gray-900">
                    Sign in
                  </a>
                </div>

                {/* Create Account */}
                <div className="flow-root">
                  <a href="#" className="-m-2 block p-2 font-medium text-gray-900">
                    Create account
                  </a>
                </div>
              </div>
            </Dialog.Panel>
          </Transition.Child>
        </div>
      </Dialog>
    </Transition.Root>
  );
};