import {ROUTER} from "./pages/routing.ts";
import {RouterProvider} from "react-router-dom";

export default function App() {
  return (
    <>
      <RouterProvider router={ROUTER}/>
    </>
  );
}