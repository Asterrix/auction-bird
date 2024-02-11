import {categoryService} from "./category.service.ts";
import {itemService} from "./items/item.service.ts";

export const apiService = {
  categories: categoryService,
  items: itemService
};