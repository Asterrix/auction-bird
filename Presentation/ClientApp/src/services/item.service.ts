import {Pageable} from "../utils/types/pagination/pageable.type.ts";
import axios from "axios";
import {environment} from "../environments/environments.development.ts";
import {ItemFilter} from "./items/item.filter.ts";

interface ItemSummary {
  id: string;
  name: string;
  initialPrice: number;
  mainImage: {
    id: number;
    imageUrl: string;
  };
}

export const itemService = {
  async getItems(pageable: Pageable, filter?: Partial<ItemFilter>) {
    const params = {
      ...pageable,
      ...filter,
    }
    
    const response = await axios.get(`${environment.apiUrl}/items`, {params: params});
    return response.data;
  }
};

export type {ItemSummary};