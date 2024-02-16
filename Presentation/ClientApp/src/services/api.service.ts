import {itemService} from "./items/item.service.ts";
import {categoryService} from "./categories/category.service.ts";
import {authService} from "./auth/auth.service.ts";

export const apiService = {
  authentication: authService,
  categories: categoryService,
  items: itemService
};