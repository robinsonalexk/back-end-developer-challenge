import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

export type ConnectionStatus = "disconnected" | "connecting" | "connected" | "reconnecting";

const connectionSlice = createSlice({
  name: "connection",
  initialState: { status: "disconnected" as ConnectionStatus },
  reducers: {
    statusChanged: (state, action: PayloadAction<ConnectionStatus>) => {
      state.status = action.payload;
    },
  },
});

export const { statusChanged } = connectionSlice.actions;
export default connectionSlice.reducer;

export const selectConnectionStatus = (s: RootState) => s.connection.status;
export const selectIsConnected = (s: RootState) => s.connection.status === "connected";