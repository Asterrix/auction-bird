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

interface ItemInfo {
  id: string;
  name: string;
  description: string;
  initialPrice: number;
  timeLeft: string;
  isActive: boolean;
  images: {
    id: number;
    imageUrl: string;
  }[];
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
  },

  async getItem(id: string) {
    const response = await axios.get(`${environment.apiUrl}/items/${id}`);
    return response.data;
  }
};

export type {ItemSummary, ItemInfo};