import axios from "axios";
import {environment} from "../../environments/environment.ts";

interface Category {
  id: number;
  name: string;
  subcategories: {
    id: number;
    name: string;
  }[];
}

const categoryService = {
  async getCategories() {
    const response = await axios.get(`${environment.apiUrl}/categories`);
    return response.data;
  }
};

export {categoryService};

export type {Category};