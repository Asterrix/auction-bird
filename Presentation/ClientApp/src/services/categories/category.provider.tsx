import React, {createContext, useEffect, useState} from "react";
import {Category} from "./category.service.ts";
import {apiService} from "../api.service.ts";

interface CategoryContextType {
  categories: Category[];
}

const CategoriesContext = createContext<CategoryContextType>({categories: []} as CategoryContextType);

const CategoryProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
  const [categories, setCategories] = useState<Category[]>([]);

  useEffect(() => {
    apiService.categories.getCategories()
      .then((categories: Category[]) => {
        setCategories(categories);
      })
      .catch((error: Error) => {
        console.error(error);
      });
  }, []);

  return (
    <CategoriesContext.Provider value={{categories}}>
      {children}
    </CategoriesContext.Provider>
  );
};

export {CategoriesContext, CategoryProvider};