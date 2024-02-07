export type ApiResponseHandler<T> = {
  onSuccess: (response: T) => void;
  onError: (error: Error) => void;
  onCompletion?: () => void;
};

export const apiCaller = async <T>(
  apiCallMethod: Promise<T>,
  responseHandler: ApiResponseHandler<T>
) => {
  try {
    const response: T = await apiCallMethod;
    responseHandler.onSuccess(response);
  } catch (error) {
    responseHandler.onError(error as Error);
  } finally {
    if (responseHandler.onCompletion) {
      responseHandler.onCompletion();
    }
  }
};