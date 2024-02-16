import {useForm} from "react-hook-form";
import {ExclamationCircleIcon} from "@heroicons/react/20/solid";
import {classNames} from "../../utils/tailwind/class-names.utils.ts";
import {SignUp} from "../../services/auth/auth.service.ts";
import {apiService} from "../../services/api.service.ts";
import {environment} from "../../environments/environment.ts";

export const SignupPage = () => {
  const {
    register,
    handleSubmit,
    formState: {errors}
  } = useForm();

  return (
    <div className="flex min-h-full flex-1 flex-col justify-center bg-slate-50 py-28 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <img
          className="mx-auto h-10 w-auto"
          src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600"
          alt="Your Company"
        />
        <h2 className="mt-6 text-center text-2xl font-bold leading-9 tracking-tight text-gray-900">
          Sign up for an account
        </h2>
      </div>

      <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-[480px]">
        <div className="bg-white px-6 py-12 shadow sm:rounded-lg sm:px-12">
          <form className="space-y-6" onSubmit={
            handleSubmit((data) => {
              const signUpForm: SignUp = {
                clientId: environment.clientId,
                firstName: data.firstName,
                lastName: data.lastName,
                email: data.email,
                password: data.password,
                termsAndConditions: data.termsAndConditions,
                isOver18: data.ageVerification
              };

              // #TODO: Handle the response
              apiService.authentication.signUp(signUpForm)
                .then((response) => {
                  console.log(response);
                }).catch((error) => {
                console.error(error);
              });
            })
          }>

            <div className="grid grid-cols-2 gap-6">
              <div>
                <label htmlFor="firstName" className="block text-sm font-medium leading-6 text-gray-900">
                  First name
                </label>
                <div className="relative mt-2 rounded-md shadow-sm">
                  <input
                    id="firstName"
                    type="text"
                    required
                    placeholder="John"
                    {...register("firstName", {
                      required: true,
                      minLength: {
                        value: 3,
                        message: "First name must be at least 3 characters long."
                      },
                      maxLength: {
                        value: 20,
                        message: "First name must be at most 20 characters long."
                      },
                      pattern: {
                        value: /^\p{L}+$/u,
                        message: "First name must consist of alphabetical letters only."
                      }
                    })}
                    className={classNames(errors.firstName ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                    )}
                  />
                  {errors.firstName && (
                    <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                      <ExclamationCircleIcon className="h-5 w-5 text-red-500" aria-hidden="true"/>
                    </div>
                  )}
                </div>
                {errors.firstName && (
                  <p className="mt-2 text-sm text-red-600">
                    {errors.firstName.message}
                  </p>
                )}
              </div>

              <div>
                <label htmlFor="lastName" className="block text-sm font-medium leading-6 text-gray-900">
                  Last name
                </label>
                <div className="relative mt-2 rounded-md shadow-sm">
                  <input
                    id="lastName"
                    type="text"
                    placeholder="Doe"
                    required
                    {...register("lastName", {
                      required: true,
                      minLength: {
                        value: 3,
                        message: "Last name must be at least 3 characters long."
                      },
                      maxLength: {
                        value: 32,
                        message: "Last name must be at most 32 characters long."
                      },
                      pattern: {
                        value: /^\p{L}+$/u,
                        message: "Last name must consist of alphabetical letters only."
                      }
                    })}
                    className={classNames(errors.lastName ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                      "block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                    )}
                  />
                  {errors.lastName && (
                    <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                      <ExclamationCircleIcon className="h-5 w-5 text-red-500" aria-hidden="true"/>
                    </div>
                  )}
                </div>
                {errors.lastName && (
                  <p className="mt-2 text-sm text-red-600">
                    {errors.lastName.message}
                  </p>
                )}
              </div>
            </div>

            <div>
              <label htmlFor="email" className="block text-sm font-medium leading-6 text-gray-900">
                Email
              </label>
              <div className="relative mt-2 rounded-md shadow-sm">
                <input
                  id="email"
                  type="text"
                  required
                  placeholder="john@doe.com"
                  {...register("email", {
                    required: true, pattern: {
                      value: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
                      message: "Invalid email format."
                    }
                  })}
                  className={classNames(errors.email ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                    "block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                  )}
                />
                {errors.email && (
                  <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                    <ExclamationCircleIcon className="h-5 w-5 text-red-500" aria-hidden="true"/>
                  </div>
                )}
              </div>
              {errors.email && (
                <p className="mt-2 text-sm text-red-600">
                  {errors.email.message}
                </p>
              )}
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium leading-6 text-gray-900">
                Password
              </label>
              <div className="relative mt-2 rounded-md shadow-sm">
                <input
                  id="password"
                  type="password"
                  required
                  placeholder="********"
                  {...register("password", {
                    required: true,
                    pattern: {
                      value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$/,
                      message: "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."
                    },
                  })}
                  className={classNames(errors.password ? "ring-red-500 focus:ring-red-500" : "ring-gray-300 focus:ring-indigo-600",
                    "block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                  )}
                />
                {errors.password && (
                  <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center pr-3">
                    <ExclamationCircleIcon className="h-5 w-5 text-red-500" aria-hidden="true"/>
                  </div>
                )}
              </div>
              {errors.password && (
                <p className="mt-2 text-sm text-red-600">
                  {errors.password.message}
                </p>
              )}
            </div>

            <div className="space-y-2">
              <div className="flex items-center">
                <input
                  id="termsAndConditions"
                  type="checkbox"
                  required
                  {...register("termsAndConditions", {required: true})}
                  className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-600"
                />
                <label htmlFor="terms-and-conditions"
                       className="ml-3 block text-sm leading-6 font-medium text-gray-900">
                  I agree to terms and conditions
                </label>
              </div>

              <div className="flex items-center">
                <input
                  id="ageVerification"
                  type="checkbox"
                  required
                  {...register("ageVerification", {required: true})}
                  className="h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-600"
                />
                <label htmlFor="age-verification" className="ml-3 block text-sm leading-6 font-medium text-gray-900">
                  I am 18 years or older
                </label>
              </div>
            </div>

            <div>
              <button
                type="submit"
                className="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold leading-6 text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
              >
                Sign up
              </button>
            </div>
          </form>
        </div>

        <p className="mt-10 text-center text-sm text-gray-500">
          Already have an account?{" "}
          <a href="#" className="font-semibold leading-6 text-indigo-600 hover:text-indigo-500">
            Sign in
          </a>
        </p>
      </div>
    </div>
  );
};