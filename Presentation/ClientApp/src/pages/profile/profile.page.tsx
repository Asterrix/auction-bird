import {classNames} from "../../utils/tailwind/class-names.utils.ts";
import {ProfilePageActiveSection} from "./profile.page.active.section.tsx";
import {ProfilePageSoldSection} from "./profile.page.sold.section.tsx";
import {useNavigate, useParams} from "react-router-dom";
import {ProfilePageBidsSection} from "./profile.page.bids.section.tsx";

const tabs = [
  {name: "Active", href: "active", Component: ProfilePageActiveSection},
  {name: "Sold", href: "sold", Component: ProfilePageSoldSection},
  {name: "Placed Bids", href: "bids", Component: ProfilePageBidsSection},
];

export const ProfilePage = () => {
  const {section} = useParams();
  const ActiveTabComponent = tabs.find((tab) => tab.href === section)?.Component;
  const activeTab = tabs.find((tab) => tab.href === section);
  const navigate = useNavigate();
  
  const handleAddNewItem = () => {
    navigate("/profile/add-new-item");
  }

  return (
    <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 pt-24">
      <div className="border-b border-gray-200 pb-5 sm:pb-0">
        <div className="mt-4 relative">
          <div className="flex md:absolute md:right-0 md:bottom-1/2">
            <button
              onClick={handleAddNewItem}
              type="button"
              className="ml-3 inline-flex items-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
            >
              Add new item
            </button>
          </div>
          <div className="sm:hidden">
            <label htmlFor="current-tab" className="sr-only">
              Select a tab
            </label>
            <select
              id="current-tab"
              name="current-tab"
              className="block w-full rounded-md border-0 py-1.5 pl-3 pr-10 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600"
            >
              {tabs.map((tab) => (
                <option key={tab.name}>{tab.name}</option>
              ))}
            </select>
          </div>
          <div className="hidden sm:block">
            <nav className="-mb-px flex space-x-8">
              {tabs.map((tab) => (
                <a
                  key={tab.name}
                  href={tab.href}
                  className={classNames(
                    activeTab === tab
                      ? "border-indigo-500 text-indigo-600"
                      : "border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700",
                    "whitespace-nowrap border-b-2 px-1 pb-4 text-sm font-medium"
                  )}
                  aria-current={activeTab === tab ? "page" : undefined}
                >
                  {tab.name}
                </a>
              ))}
            </nav>
          </div>
        </div>

      </div>
      <div className="overflow-hidden">
        <div className="flex flex-wrap items-center sm:flex-nowrap">
          <div className="flow-root overflow-hidden w-full">
            <div className="mx-auto max-w-7xl">
              {ActiveTabComponent && <ActiveTabComponent/>}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
