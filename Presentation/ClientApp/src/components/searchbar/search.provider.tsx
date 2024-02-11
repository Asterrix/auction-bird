import React, {createContext, useState} from "react";

interface SearchContextType {
  search: string;
  setSearch: React.Dispatch<React.SetStateAction<string>>;
}

export const SearchContext = createContext<SearchContextType>({} as SearchContextType);

export const SearchProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
  const [search, setSearch] = useState("");

  return (
    <SearchContext.Provider value={{search, setSearch}}>
      {children}
    </SearchContext.Provider>
  );
};