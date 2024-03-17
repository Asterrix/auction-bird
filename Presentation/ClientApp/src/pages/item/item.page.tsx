import {classNames} from "../../utils/tailwind/class-names.utils.ts";
import {useNavigate, useParams} from "react-router-dom";
import {SetStateAction, useContext, useEffect, useState} from "react";
import {ItemInfo} from "../../services/items/item.service.ts";
import {apiService} from "../../services/api.service.ts";
import {FieldValues, useForm} from "react-hook-form";
import {Bid} from "../../services/bidding/bidding.service.ts";
import {environment} from "../../environments/environment.ts";
import {userContext} from "../../services/auth/user.provider.tsx";
import {SuccessAlert} from "../../components/success-alert/success-alert.component.tsx";
import {FailureAlert} from "../../components/failure-alert/failure-alert.component.tsx";
import {WarningAlert} from "../../components/warning-alert/warning-alert.component.tsx";

export const ItemPage = () => {
    const {
      register,
      handleSubmit,
    } = useForm();

    const navigate = useNavigate();

    const {user} = useContext(userContext);
    const [item, setItem] = useState<ItemInfo | null>(null);

    // Image slider
    const [currentIndex, setCurrentIndex] = useState(0);
    const handleTabClick = (index: SetStateAction<number>) => {
      setCurrentIndex(index);
    };

    // Alerts
    const [successfulBid, setSuccessfulBid] = useState(false);
    const [warningBid, setWarningBid] = useState(false);
    const [failedBid, setFailedBid] = useState(false);

    // Highest bidder
    const [displayFinalizePurchaseBtn, setDisplayFinalizePurchaseBtn] = useState(false);
    
    // Fetch item
    const {id} = useParams();
    useEffect(() => {
      if (id === undefined) return;

      apiService.items.getItem(id)
        .then((response: ItemInfo) => {
          setItem(response);
        })
        .catch((error) => {
          console.error(error);
        });

    }, [id]);

    useEffect(() => {
      if (item && item.auctionFinished && item.isActive && user) {
        apiService.bidding.findHighestBidder(item.id)
          .then((highestBidder) => {
            if (highestBidder === user.username) {
              setDisplayFinalizePurchaseBtn(true);
            }
          });
      }
    }, [item, user]);

    // Submit bid
    const submitForm = (data: FieldValues) => {
      setWarningBid(false);
      setSuccessfulBid(false);
      setFailedBid(false);

      if (data.offer <= item!.currentPrice) {
        setWarningBid(true);
        return;
      }

      const bidForm: Bid = {
        clientId: environment.clientId,
        itemId: item!.id,
        bidAmount: parseFloat(data.offer)
      };

      apiService.bidding.makeBid(bidForm)
        .then(() => {
          setSuccessfulBid(true);
          setItem({...item!, currentPrice: bidForm.bidAmount});
        })
        .catch(() => {
          setFailedBid(true);
        });
    };

    // Finalize purchase
    const finalizePurchase = () => {
      if (!user || !item) return;

      apiService.payments.createPaymentSession(user.username, item.id)
        .then((response) => window.location.href = response.url);
    };

    return (
      <>
        {item && (
          <div className="bg-white pt-12">
            <main className="mx-auto max-w-7xl sm:px-6 sm:pt-16 lg:px-8 border-b border-b-gray-200 pb-12">
              {successfulBid && (
                <div className="mb-5">
                  <SuccessAlert message="Congratulations! You are the highest bidder!"/>
                </div>
              )}
              {warningBid && (
                <div className="mb-5">
                  <WarningAlert message="There are higher bids than yours. You could give it a second try!"/>
                </div>
              )}
              {failedBid && (
                <div className="mb-5">
                  <FailureAlert
                    message="Unfortunatly we were unable to proccess your bid. Please try again! We apologise for any inconvenience."/>
                </div>
              )}
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

                    {/* Item details */}
                    <div className="grid gap-6 mt-6">
                      <div className="grid grid-cols-1 gap-1">
                        <h2 className="font-medium text-3xl text-gray-900">Description</h2>

                        <div
                          className="space-y-6 text-base text-gray-700"
                          dangerouslySetInnerHTML={{__html: item.description}}
                        />
                      </div>

                      <div className="grid grid-cols-1 gap-1">
                        <h2 className="font-medium text-3xl text-gray-900">Auction Details</h2>

                        <div className="space-y-1 font-medium text-xl">
                          <p className="flex gap-1.5 text-indigo-500 font-medium">
                            <span className="text-gray-700">Current price:</span>
                            ${item.currentPrice}
                          </p>
                          {item.auctionStarted ? (
                            <p className="flex gap-1.5 text-indigo-500 font-medium">
                              <span className="text-gray-700">Time Left:</span>
                              {item.timeTillEnd}
                            </p>
                          ) : (
                            <p className="flex gap-1.5 text-indigo-500 font-medium">
                              <span className="text-gray-700">Auction begins in:</span>
                              {item.timeTillStart}
                            </p>
                          )}
                        </div>
                      </div>

                      {user && item.auctionStarted && !item.auctionFinished && (item.ownerId !== user.subject) && (
                        <form className="flex gap-3 items-center" onSubmit={
                          handleSubmit((data) => {
                            submitForm(data);
                          })
                        }>
                          <div className="flex flex-col w-full">
                            <div className="grid items-baseline gap-3 lg:grid-cols-2">
                              <div className="relative mt-2 rounded-md shadow-sm">
                                <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                                  <span className="text-gray-500 sm:text-sm">$</span>
                                </div>
                                <input
                                  type="text"
                                  id="offer"
                                  className="block w-full rounded-md border-0 py-2.5 pl-7 pr-12 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                                  placeholder="0.00"
                                  {...register("offer", {
                                    required: true,
                                    pattern: {
                                      value: /^[0-9]+(\.[0-9]{1,2})?$/,
                                      message: "Invalid number"
                                    },
                                    onChange: (e) => {
                                      e.target.value = e.target.value.replace(/[^0-9.]/g, "");
                                      e.target.value = e.target.value.replace(/(\..*)\./g, "$1");
                                      e.target.value = e.target.value.replace(/(\.[0-9]{2})./g, "$1");
                                    }
                                  })}
                                  aria-describedby="price-currency"
                                />
                                <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                                  <span className="text-gray-500 sm:text-sm" id="price-currency">USD</span>
                                </div>
                              </div>

                              <button
                                type="submit"
                                className="flex w-full lg:max-w-32 items-center justify-center rounded-md border border-transparent bg-indigo-600 py-2.5 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 focus:ring-offset-gray-50">
                                Bid
                              </button>
                            </div>
                          </div>
                        </form>
                      )}

                      {displayFinalizePurchaseBtn && (
                        <form className="flex gap-3 items-center" onSubmit={
                          handleSubmit(() => finalizePurchase())
                        }>
                          <div className="flex flex-col w-full">
                            <div className="grid items-baseline gap-3 grid-cols-1">
                              <button
                                type="submit"
                                className="flex w-full lg:max-w-48 items-center justify-center rounded-md border border-transparent bg-indigo-600 py-2.5 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 focus:ring-offset-gray-50">
                                Finalize purchase
                              </button>
                            </div>
                          </div>
                        </form>
                      )}

                      {!user && item.auctionStarted && !item.auctionFinished && (
                        <button
                          onClick={() => {
                            navigate("/signin", {state: {from: location.pathname}});
                          }}
                          type="submit"
                          className="flex w-full lg:max-w-32 items-center justify-center rounded-md border border-transparent bg-indigo-600 py-2.5 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 focus:ring-offset-gray-50">
                          Sign in to bid
                        </button>
                      )}
                    </div>

                  </div>
                </div>

              </div>
            </main>

          </div>
        )}
      </>
    );
  }
;
