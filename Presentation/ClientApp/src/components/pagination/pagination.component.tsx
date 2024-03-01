import {ArrowLongLeftIcon, ArrowLongRightIcon} from "@heroicons/react/20/solid";
import {classNames} from "../../utils/tailwind/class-names.utils.ts";
import {FC} from "react";
import {Pageable} from "../../utils/types/pagination/pageable.type.ts";

export const Pagination: FC<{ pageable: Pageable, totalPages: number }> = ({pageable, totalPages}) => {
  const pageHref = (page: number) => `?page=${page}&size=${pageable.size}`;

  return (
    <nav className="flex items-center justify-between border-t border-gray-200 px-4 sm:px-0">
      <div className="-mt-px flex w-0 flex-1">
        {pageable.page !== 1 && (
          <a
            href={pageHref(pageable.page - 1)}
            className="inline-flex items-center border-t-2 border-transparent pr-1 pt-4 text-sm font-medium text-gray-500 hover:border-gray-300 hover:text-gray-700"
          >
            <ArrowLongLeftIcon className="mr-3 h-5 w-5 text-gray-400" aria-hidden="true"/>
            Previous
          </a>
        )}
      </div>
      <div className="hidden md:-mt-px md:flex">
        {Array.from({length: totalPages}, (_, i) => (
          <a
            key={i}
            href={pageHref(i + 1)}
            className={classNames(
              i + 1 === pageable.page
                ? "border-indigo-500 text-indigo-600"
                : "border-transparent text-gray-500 hover:text-gray-700",
              "inline-flex items-center border-t-2 px-4 pt-4 text-sm font-medium text-gray-500 hover:border-gray-300 hover:text-gray-700")}
          >
            {i + 1}
          </a>
        ))}
      </div>
      <div className="-mt-px flex w-0 flex-1 justify-end">
        {pageable.page !== totalPages && (
          <a
            href={pageHref(pageable.page + 1)}
            className="inline-flex items-center border-t-2 border-transparent pl-1 pt-4 text-sm font-medium text-gray-500 hover:border-gray-300 hover:text-gray-700"
          >
            Next
            <ArrowLongRightIcon className="ml-3 h-5 w-5 text-gray-400" aria-hidden="true"/>
          </a>
        )}
      </div>
    </nav>
  );
};