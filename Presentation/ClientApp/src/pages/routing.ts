import {createBrowserRouter} from "react-router-dom";
import {HomePage} from "./home/home.page.tsx";
import {MarketplacePage} from "./marketplace/marketplace.page.tsx";

export const ROUTER = createBrowserRouter([
  {
    path: "/",
    element: HomePage()
  },
  {
    path: "/marketplace",
    Component: MarketplacePage
  }
]);