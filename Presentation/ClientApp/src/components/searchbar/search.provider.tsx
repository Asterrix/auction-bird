import React, {createContext, useState} from "react";
import {useDebounce} from "../../utils/debounce/debounce.ts";

interface SearchContextType {
  search: string;
  debouncedSearch: string;
  setSearch: React.Dispatch<React.SetStateAction<string>>;
}

export const SearchContext = createContext<SearchContextType>({} as SearchContextType);

export const SearchProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, 300);

  return (
    <SearchContext.Provider value={{search, debouncedSearch, setSearch}}>
      {children}
    </SearchContext.Provider>
  );
};