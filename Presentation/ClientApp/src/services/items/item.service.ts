import axios from "axios";
import {Pageable} from "../../utils/types/pagination/pageable.type.ts";
import {ItemFilter} from "./item.filter.ts";
import {environment} from "../../environments/environment.ts";

interface ItemSummary {
  id: string;
  name: string;
  initialPrice: number;
  mainImage: {
    id: number;
    imageUrl: string;
  };
}

interface Param {
  page: number;
  size: number;
  search?: string;
  categories?: string;
}

export const itemService = {
  async getItems(pageable: Pageable, filter?: Partial<ItemFilter>) {
    const params: Param = {
      page: pageable.page,
      size: pageable.size,
      search: filter?.search || undefined,
      categories: filter?.categories?.flat().join(",") || undefined
    };

    const response = await axios.get(`${environment.apiUrl}/items`, {params: params});
    return response.data;
  }
};

export type {ItemSummary};