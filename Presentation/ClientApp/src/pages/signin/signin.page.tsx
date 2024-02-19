import {useForm} from "react-hook-form";
import {SignIn} from "../../services/auth/auth.service.ts";
import {environment} from "../../environments/environment.ts";
import {apiService} from "../../services/api.service.ts";
import {useCallback, useContext, useState} from "react";
import {XCircleIcon} from "@heroicons/react/20/solid";
import {userContext} from "../../services/auth/user.provider.tsx";

type Error = {
  present: boolean;
  message: string;
}

export const SigninPage = () => {
  const {
    register,
    handleSubmit,
  } = useForm();

  const {updateUser} = useContext(userContext);
  const [error, setError] = useState<Error>({present: false, message: ""});

  const resetError = useCallback(() => {
    if (error.present) {
      const timer = setTimeout(() => {
        setError({present: false, message: ""});
      }, 3000);
      return () => clearTimeout(timer);
    }
  }, [error.present]);

  return (
    <div className="flex min-h-full flex-1 flex-col justify-center bg-slate-50 py-28 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <img
          className="mx-auto h-10 w-auto"
          src="https://tailwindui.com/img/logos/mark.svg?color=indigo&shade=600"
          alt="Your Company"
        />
        <h2 className="mt-6 text-center text-2xl font-bold leading-9 tracking-tight text-gray-900">
          Sign in to your account
        </h2>
      </div>

      <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-[480px]">
        <div className="bg-white px-6 py-12 shadow sm:rounded-lg sm:px-12">
          {error.present && (
            <div className="rounded-md bg-red-50 p-4 mb-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <XCircleIcon className="h-5 w-5 text-red-400" aria-hidden="true"/>
                </div>
                <div className="ml-3">
                  <h3 className="text-sm font-medium text-red-800">Error occured while processing your request.</h3>
                  <div className="mt-2 text-sm text-red-700">
                    <p>{error.message}</p>
                  </div>
                </div>
              </div>
            </div>
          )}
          <form className="space-y-6" onSubmit={
            handleSubmit((data) => {
              const signInForm: SignIn = {
                clientId: environment.clientId,
                email: data.email,
                password: data.password,
              };

              apiService.authentication.signIn(signInForm)
                .then(() => {
                  window.location.href = "/";
                  updateUser();
                })
                .catch((error) => {
                  const message = error.response.data.detail;
                  setError({present: true, message: message});
                });
            })
          }>

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
                  onFocus={resetError}
                  {...register("email", {required: true})}
                  className="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-gray-300 focus:ring-indigo-600 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                />
              </div>
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
                  onFocus={resetError}
                  {...register("password", {required: true})}
                  className="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-gray-300 focus:ring-indigo-600 ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset sm:text-sm sm:leading-6"
                />
              </div>
            </div>

            <div>
              <button
                type="submit"
                className="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm font-semibold leading-6 text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
              >
                Sign in
              </button>
            </div>
          </form>
        </div>

        <p className="mt-10 text-center text-sm text-gray-500">
          Don't have an account?{" "}
          <a href="/signup" className="font-semibold leading-6 text-indigo-600 hover:text-indigo-500">
            Sign up
          </a>
        </p>
      </div>
    </div>
  );
};