import {MagnifyingGlassIcon} from "@heroicons/react/24/outline";
import {useContext} from "react";
import {SearchContext} from "./search.provider.tsx";

export const SearchBar = () => {
  const {search, setSearch} = useContext(SearchContext);

  return (
    <div className="mx-6 flex flex-1 items-center justify-center lg:mr-6 lg:ml-6 lg:justify-end">
      <div className="w-full max-w-lg lg:max-w-xs">
        <label htmlFor="search" className="sr-only">
          Search
        </label>
        <div className="relative">
          <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
            <MagnifyingGlassIcon className="h-5 w-5 text-gray-400" aria-hidden="true"/>
          </div>
          <input
            id="search"
            name="search"
            className="block w-full rounded-md border-0 bg-white pr-3 pl-10 placeholder:text-gray-400 text-gray-900 ring-1 ring-inset ring-gray-300 py-1.5 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
            placeholder="Search"
            type="search"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
      </div>
    </div>
  );
};