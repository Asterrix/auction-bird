import {ExclamationTriangleIcon} from "@heroicons/react/20/solid";
import React from "react";

export const WarningAlert: React.FC<{ message: string }> = ({message}) => {
  return (
    <div className="rounded-md bg-yellow-50 p-4">
      <div className="flex">
        <div className="flex-shrink-0">
          <ExclamationTriangleIcon className="h-5 w-5 text-yellow-400" aria-hidden="true"/>
        </div>
        <div className="ml-3">
          <p className="text-sm font-medium text-yellow-800">{message}</p>
        </div>
      </div>
    </div>
  );
};