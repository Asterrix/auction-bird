export const Spinner = () => (
  <div className="relative">
    <div className="absolute right-1/2 bottom-1/2 transform translate-x-1/2 translate-y-1/2">
      <div className="p-2 bg-gradient-to-tr animate-spin from-violet-500 to-indigo-700 via-purple-700 rounded-full">
        <div className="bg-white rounded-full">
          <div className="w-9 h-9 rounded-full"></div>
        </div>
      </div>
    </div>
  </div>
); 