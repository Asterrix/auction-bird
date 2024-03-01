import {ClipboardDocumentListIcon, PlusIcon} from "@heroicons/react/24/outline";
import {useCallback, useContext, useEffect, useState} from "react";
import {userContext} from "../../services/auth/user.provider.tsx";
import {Page} from "../../utils/types/pagination/page.type.ts";
import {ActiveItem} from "../../services/user/user.service.ts";
import {apiService} from "../../services/api.service.ts";
import {useNavigate} from "react-router-dom";
import {Pagination} from "../../components/pagination/pagination.component.tsx";

export const ProfilePageActiveSection = () => {
  const {user} = useContext(userContext);
  const [activeItems, setActiveItems] = useState<Page<ActiveItem>>();
  const [pageable, setPageable] = useState({page: 1, size: 9});
  const navigate = useNavigate();

  const handleItemClick = (itemId: string) => {
    navigate(`/marketplace/item/${itemId}`);
  };

  const fetchActiveItems = useCallback(() => {
    if (!user) return;

    apiService.users.listActiveItems(user.username, pageable)
      .then((response) => {
        setActiveItems(response);
      })
      .catch((error) => console.error(error));
  }, [user, pageable]);

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);

    const page = parseInt(params.get("page") ?? String(pageable.page));
    const size = parseInt(params.get("size") ?? String(pageable.size));

    setPageable(prevPageable => ({...prevPageable, page, size}));
  }, []);

  useEffect(() => {
    fetchActiveItems();
  }, [user, pageable]);

  return (
    <>
      {activeItems?.isEmpty ? (
        <div className="text-center my-32">
          <ClipboardDocumentListIcon className="mx-auto h-12 w-12 text-gray-400" aria-hidden="true"/>
          <h3 className="mt-2 text-sm font-semibold text-gray-900">No items were found.</h3>
          <p className="mt-1 text-sm text-gray-500">Start placing your items up for auction!</p>
          <div className="mt-6">
            <button
              type="button"
              className="inline-flex items-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
            >
              <PlusIcon className="-ml-0.5 mr-1.5 h-5 w-5" aria-hidden="true"/>
              Add Item
            </button>
          </div>
        </div>
      ) : (
        <>
          <table className="w-full text-left px-4 sm:px-6 lg:px-8">
            <thead className="bg-white">
            <tr>
              <th scope="col" className="relative isolate py-3.5 pr-3 text-left text-sm font-semibold text-gray-900">
                Name
                <div className="absolute inset-y-0 right-full -z-10 w-screen border-b border-b-gray-200"/>
                <div className="absolute inset-y-0 left-0 -z-10 w-screen border-b border-b-gray-200"/>
              </th>
              <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                Time left
              </th>
              <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                Initial price
              </th>
              <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                No. bids
              </th>
              <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                Highest bid
              </th>
            </tr>
            </thead>
            <tbody>
            {activeItems?.elements.map((item) => (
              <tr key={item.id}
                  className="hover:bg-slate-50 hover:cursor-pointer"
                  onClick={() => handleItemClick(item.id)}
              >
                <td className="py-4  md:pl-6">
                  <div className="flex items-center gap-x-4">
                    <img
                      src={item.mainImage.imageUrl}
                      alt={item.name + " image"}
                      className="h-12 w-12 rounded-full bg-gray-800 object-cover"/>
                    <div className="truncate text-sm font-medium leading-6 text-gray-900">{item.name}</div>
                  </div>
                </td>
                <td className="px-3 py-4 text-sm text-gray-500">{item.timeLeft}</td>
                <td className="px-3 py-4 text-sm text-gray-500">${item.initialPrice}</td>
                <td className="px-3 py-4 text-sm text-gray-500">{item.numberOfBids}</td>
                <td className="px-3 py-4 text-sm text-gray-500">${item.highestBid}</td>
              </tr>
            ))}
            </tbody>
          </table>
          <Pagination pageable={pageable} totalPages={activeItems?.totalPages ?? 0}/>
        </>
      )}
    </>
  );
};
