import { createAsyncThunk, createEntityAdapter, createSlice, type PayloadAction } from "@reduxjs/toolkit";
import type { Character } from "../types/character";
import { api } from "../api";

const adapter = createEntityAdapter<Character, string>({
  selectId: (character: Character) => character.name
});

export const fetchCharacters = createAsyncThunk("charaters/fetch", () => api.getCharacters());

const characersSlice = createSlice({
  name: "characters",
  initialState: adapter.getInitialState({
    status: "idle" as "idle" | "loading" | "succeeded" | "failed",
    error: null as string | null,
  }),
  reducers: {
    characterUpserted: (state, action: PayloadAction<Character>) => adapter.upsertOne(state, action.payload),
    characterRemoved: (state, action: PayloadAction<string>) => adapter.removeOne(state, action.payload),
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCharacters.pending, (state) => {
        state.status = "loading";
        state.error = null;
      })
      .addCase(fetchCharacters.fulfilled, (state, action) => {
        state.status = "succeeded";
        adapter.setAll(state, action.payload);
      })
      .addCase(fetchCharacters.rejected, (state, action) => {
        state.status = "failed";
        state.error = action.error.message ?? "Failed to load items";
      });
  },
});

export const { characterUpserted, characterRemoved } = characersSlice.actions;
export const charactersSelectors = adapter.getSelectors((s: { characters: ReturnType<typeof characersSlice.reducer> }) => s.characters);
export default characersSlice.reducer;
