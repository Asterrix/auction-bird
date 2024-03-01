import {PlusIcon, ShoppingCartIcon} from "@heroicons/react/24/outline";

const items = [
  {name: "Bag", timeLeft: "2 days", initialPrice: "$50", numBids: "10", highestBid: "$100"}
];

export const ProfilePageBidsSection = () => {
  return (
    <>
      {items.length > 0 ? (
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
          {items.map((item) => (
            <tr key={item.initialPrice} className="hover:bg-slate-50 hover:cursor-pointer">
              <td className="py-4  md:pl-6">
                <div className="flex items-center gap-x-4">
                  <img
                    src="https://images.unsplash.com/photo-1438761681033-6461ffad8d80?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=facearea&facepad=2&w=256&h=256&q=80"
                    alt="" className="h-8 w-8 rounded-full bg-gray-800"/>
                  <div className="truncate text-sm font-medium leading-6 text-gray-900">{item.name}</div>
                </div>
              </td>
              <td className="px-3 py-4 text-sm text-gray-500">{item.timeLeft}</td>
              <td className="px-3 py-4 text-sm text-gray-500">{item.initialPrice}</td>
              <td className="px-3 py-4 text-sm text-gray-500">{item.numBids}</td>
              <td className="px-3 py-4 text-sm text-gray-500">{item.highestBid}</td>
            </tr>
          ))}
          </tbody>
        </table>
      ) : (
        /* No Items */
        <div className="text-center my-32">
          <ShoppingCartIcon className="mx-auto h-12 w-12 text-gray-400" aria-hidden="true"/>
          <h3 className="mt-2 text-sm font-semibold text-gray-900">No items were found.</h3>
          <p className="mt-1 text-sm text-gray-500">Start bidding for exiting items!</p>
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
      )}
    </>
  );
};
