import {itemService} from "./items/item.service.ts";
import {categoryService} from "./categories/category.service.ts";

export const apiService = {
  categories: categoryService,
  items: itemService
};