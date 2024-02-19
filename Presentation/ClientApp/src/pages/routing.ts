import {createBrowserRouter} from "react-router-dom";
import {HomePage} from "./home/home.page.tsx";
import {MarketplacePage} from "./marketplace/marketplace.page.tsx";
import {ItemPage} from "./item/item.page.tsx";
import {SignupPage} from "./signup/signup.page.tsx";
import {SigninPage} from "./signin/signin.page.tsx";

export const ROUTER = createBrowserRouter([
  {
    path: "/",
    element: HomePage()
  },
  {
    path: "/marketplace",
    Component: MarketplacePage
  },
  {
    path: "/marketplace/item/:id",
    Component: ItemPage
  },
  {
    path: "/signup",
    Component: SignupPage
  },
  {
    path: "/signin",
    Component: SigninPage
  }
]);