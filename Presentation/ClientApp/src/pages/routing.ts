import {createBrowserRouter} from "react-router-dom";
import {HomePage} from "./home/home.page.tsx";

export const ROUTER = createBrowserRouter([
    {
        path: "/",
        element: HomePage()
    }
]);