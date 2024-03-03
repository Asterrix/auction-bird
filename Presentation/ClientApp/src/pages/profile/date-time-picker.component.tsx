import {LocalizationProvider} from "@mui/x-date-pickers/LocalizationProvider";
import {StaticDateTimePicker} from "@mui/x-date-pickers/StaticDateTimePicker";
import {AdapterDateFns} from "@mui/x-date-pickers/AdapterDateFnsV3";

export const DateTimePicker = ({onAccept, onClose}) => {
  const today: Date = new Date();
  const minDate: Date = new Date(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate() + 1);
  const maxDate: Date = new Date(today.getUTCFullYear(), today.getUTCMonth() + 3, today.getUTCDate());

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <StaticDateTimePicker
        orientation="landscape"
        minDateTime={minDate}
        maxDateTime={maxDate}
        timezone="UTC"
        onAccept={onAccept}
        onClose={onClose}
      />
    </LocalizationProvider>
  );
};