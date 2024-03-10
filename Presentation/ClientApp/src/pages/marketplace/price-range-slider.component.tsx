import {Slider} from "@mui/material";
import {FC, useEffect, useState} from "react";
import {useDebounce} from "../../utils/debounce/debounce.ts";

export const PriceRangeSlider: FC<{
  currentPriceRange: [number, number],
  minPrice: number,
  maxPrice: number,
  setPriceRange: (min: number, max: number) => void
}> = ({minPrice, maxPrice, setPriceRange}) => {
  const [value, setValue] = useState([minPrice, maxPrice]);
  const debounceVal = useDebounce(value, 500);

  const displayValue = (value: number | number[]) => `$${value}`;

  useEffect(() => {
    setPriceRange(debounceVal[0], debounceVal[1]);
  }, [debounceVal]);

  return (
    <div className="border-b py-6">
      <h3 className="grid gap-3">
        <span className="text-sm font-medium text-gray-900">Price range</span>
        <Slider
          getAriaLabel={() => "Price range slider"}
          sx={{
            color: "rgb(67 56 202)",
            width: "98%",
            justifySelf: "center"
          }}
          value={value}
          min={minPrice}
          max={maxPrice}
          onChange={(_, newValue) => setValue(newValue as number[])}
          onChangeCommitted={(event, newValue) => setValue(newValue as number[])}
          valueLabelDisplay="auto"
          getAriaValueText={displayValue}
          disableSwap
        />
      </h3>
    </div>
  );
};