import * as signalR from "@microsoft/signalr";
import { BASE_URL } from ".";
import type { AppDispatch } from "../store";
import { characterRemoved, characterUpserted, fetchCharacters } from "../slices/characterSlice";
import { statusChanged } from "../slices/connectionSlice";

const HUB_URL = `${BASE_URL}/hubs/characters`;

let connection: signalR.HubConnection | null = null;

export async function connectToHub(dispatch: AppDispatch) {
  if (connection) return;

  connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL)
    .withAutomaticReconnect()
    .build();

  connection.on("CharacterUpserted", (item) => dispatch(characterUpserted(item)));
  connection.on("CharacterRemoved", (name: string) => dispatch(characterRemoved(name)));

  connection.onreconnecting(() => dispatch(statusChanged("connecting")));
  connection.onreconnected(() => {
    dispatch(statusChanged("connected"));
    dispatch(fetchCharacters());
  });
  connection.onclose(() => dispatch(statusChanged("disconnected")));

  try {
    await connection.start();
    dispatch(statusChanged("connected"));
  } catch (err) {
    console.error("SignalR connection failed", err);
  }
}