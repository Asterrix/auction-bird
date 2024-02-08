type Page<T> = {
  elements: T[];
  totalElements: number;
  totalPages: number;
  isEmpty: boolean;
  isLastPage: boolean;
}

export type {Page};