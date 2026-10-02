import { configureStore } from "@reduxjs/toolkit";
import { connectToHub } from "../api/signalR";
import charactersReducer, { fetchCharacters } from "../slices/characterSlice";
import connectionReducer from "../slices/connectionSlice";

export const store = configureStore({
  reducer: { characters: charactersReducer, connection: connectionReducer },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export async function initializeStore() {
  await store.dispatch(fetchCharacters());
  await connectToHub(store.dispatch);
}
