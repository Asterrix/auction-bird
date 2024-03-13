import {createBrowserRouter} from "react-router-dom";
import {HomePage} from "./home/home.page.tsx";
import {MarketplacePage} from "./marketplace/marketplace.page.tsx";
import {ItemPage} from "./item/item.page.tsx";
import {SignupPage} from "./signup/signup.page.tsx";
import {SigninPage} from "./signin/signin.page.tsx";
import {ProfilePage} from "./profile/profile.page.tsx";
import {ProfilePageActiveSection} from "./profile/profile.page.active.section.tsx";
import {ProfilePageSoldSection} from "./profile/profile.page.sold.section.tsx";
import {ProfilePageBidsSection} from "./profile/profile.page.bids.section.tsx";
import {AddItemPage} from "./add-item/add-item.page.tsx";

export const ROUTER = createBrowserRouter([
  {
    path: "/",
    Component: HomePage
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
  },
  {
    path: "/profile/:section",
    Component: ProfilePage,
    children: [
      {
        path: "active",
        Component: ProfilePageActiveSection
      },
      {
        path: "sold",
        Component: ProfilePageSoldSection
      },
      {
        path: "bids",
        Component: ProfilePageBidsSection
      }
    ]
  },
  {
    path: "/profile/add-new-item",
    Component: AddItemPage
  }
]);