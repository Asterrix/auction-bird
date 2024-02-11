import {ROUTER} from "./pages/routing.ts";
import {RouterProvider} from "react-router-dom";
import {NavbarComponent} from "./components/navbar/navbar.component.tsx";
import {FooterComponent} from "./components/footer/footer.component.tsx";
import {SearchProvider} from "./components/searchbar/search.provider.tsx";

export default function App() {
  return (
    <>
      <SearchProvider>
        <NavbarComponent/>
        <RouterProvider router={ROUTER}/>
      </SearchProvider>
      <FooterComponent/>
    </>
  );
}