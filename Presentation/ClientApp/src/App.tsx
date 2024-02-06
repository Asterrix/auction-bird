import {ROUTER} from "./pages/routing.ts";
import {RouterProvider} from "react-router-dom";
import {NavbarComponent} from "./components/navbar/navbar.component.tsx";

export default function App() {
  return (
    <>
      <NavbarComponent/>
      <RouterProvider router={ROUTER}/>
    </>
  );
}