import {Pageable} from "../utils/types/pagination/pageable.type";
import {Page} from "../utils/types/pagination/page.type";

const defaultPagination: () => Pageable = () => ({
  page: 1,
  size: 9
});

const nextPage = (pageable: Pageable, page: Page<any>): Pageable => {
  if (!page.isLastPage) {
    return {page: pageable.page + 1, size: pageable.size};
  }

  return pageable;
};

const resetPagination = (): Pageable => {
  return defaultPagination();
};

export {defaultPagination, nextPage, resetPagination};