import {classNames} from "../../utils/tailwind/class-names.utils.ts";
import {useParams} from "react-router-dom";
import {SetStateAction, useEffect, useState} from "react";
import {ItemInfo} from "../../services/items/item.service.ts";
import {apiService} from "../../services/api.service.ts";

export const ItemPage = () => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [item, setItem] = useState<ItemInfo | null>(null);

  const handleTabClick = (index: SetStateAction<number>) => {
    setCurrentIndex(index);
  };

  const {id} = useParams();
  useEffect(() => {
    if (id === undefined) return;

    apiService.items.getItem(id)
      .then((response) => {
        setItem(response);
      })
      .catch((error) => {
        console.error(error);
      });

  }, [id]);

  return (
    <>
      {item && (
        <div className="bg-white pt-12">
          <main className="mx-auto max-w-7xl sm:px-6 sm:pt-16 lg:px-8 border-b border-b-gray-200 pb-12">
            <div className="mx-auto max-w-2xl lg:max-w-none">
              <div className="lg:grid lg:grid-cols-2 lg:items-start lg:gap-x-8">

                {/* Image gallery */}
                <div className="flex flex-col-reverse">
                  {/* Image selector */}
                  <div className="mx-auto mt-6 w-full max-w-2xl sm:block lg:max-w-none">
                    <div className="grid grid-cols-4 gap-6">
                      {item.images.map((image, index) => (
                        <div
                          key={image.id}
                          className="relative flex h-24 cursor-pointer items-center justify-center rounded-md bg-white text-sm font-medium uppercase text-gray-900 hover:bg-gray-50 focus:outline-none focus:ring focus:ring-opacity-50 focus:ring-offset-4"
                          onClick={() => handleTabClick(index)}
                        >
                          <span className="sr-only">{image.id}</span>
                          <span className="absolute inset-0 overflow-hidden rounded-md">
                              <img src={image.imageUrl} alt={`${item.name} image ${index}`}
                                   className="h-full w-full object-cover object-center"/>
                            </span>
                          <span
                            className={classNames(
                              index === currentIndex ? "ring-indigo-500" : "ring-transparent",
                              "pointer-events-none absolute inset-0 rounded-md ring-2 ring-offset-2"
                            )}
                            aria-hidden="true"
                          />
                        </div>
                      ))}
                    </div>
                  </div>

                  <div className="aspect-h-1 aspect-w-1 w-full">
                    <img
                      src={item.images[currentIndex].imageUrl}
                      alt={item.images[currentIndex].imageUrl}
                      className="h-full w-full object-cover object-center sm:rounded-lg"
                    />
                  </div>
                </div>

                {/* Item name */}
                <div className="mt-10 px-4 sm:mt-16 sm:px-0 lg:mt-0">
                  <h1 className="text-3xl font-bold tracking-tight text-gray-900">{item.name}</h1>

                  <div className="mt-6 grid grid-cols-1 gap-1 pb-12">
                    <h2 className="font-medium text-gray-900">Description</h2>

                    <div
                      className="space-y-6 text-base text-gray-700"
                      dangerouslySetInnerHTML={{__html: item.description}}
                    />
                  </div>

                </div>
              </div>

            </div>
          </main>

        </div>
      )}
    </>
  );
};