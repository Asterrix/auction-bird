import {itemService} from "./items/item.service.ts";
import {categoryService} from "./categories/category.service.ts";
import {authService} from "./auth/auth.service.ts";
import {biddingService} from "./bidding/bidding.service.ts";
import {userService} from "./user/user.service.ts";
import {recommendationsService} from "./recommendations/recommendations.service.ts";

export const apiService = {
  authentication: authService,
  bidding: biddingService,
  categories: categoryService,
  items: itemService,
  users: userService,
  recommendations: recommendationsService
};