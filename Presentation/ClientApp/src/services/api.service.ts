import {itemService} from "./items/item.service.ts";
import {categoryService} from "./categories/category.service.ts";
import {authService} from "./auth/auth.service.ts";
import {biddingService} from "./bidding/bidding.service.ts";

export const apiService = {
  authentication: authService,
  bidding: biddingService,
  categories: categoryService,
  items: itemService
};