import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, test, expect, vi } from "vitest";
import ActionBar from "./ActionBar";
import { DamageTypes } from "../../types/damageTypes";

const setup = () => {
  const props = {
    onDamageClick: vi.fn(),
    onHealClick: vi.fn(),
    onAddTempHpClick: vi.fn(),
  };
  render(<ActionBar {...props} />);
  return props;
};

describe("ActionBar", () => {
  test("strips non-digit characters from the damage input", async () => {
    setup();
    const input = screen.getByLabelText(/enter damage amount/i);
    await userEvent.type(input, "12ab3");
    expect(input).toHaveValue("123");
  });

  test("calls onDamageClick with amount and selected type", async () => {
    const { onDamageClick } = setup();
    await userEvent.type(screen.getByLabelText(/enter damage amount/i), "5");
    await userEvent.selectOptions(
      screen.getByLabelText(/select damage type/i),
      DamageTypes.Fire
    );
    await userEvent.click(screen.getByRole("button", { name: /deal damage/i }));
    expect(onDamageClick).toHaveBeenCalledWith(5, DamageTypes.Fire);
  });

  test("temp HP button with empty input does not send NaN", async () => {
    const { onAddTempHpClick } = setup();
    await userEvent.click(
      screen.getByRole("button", { name: /set temporary hp/i })
    );
    expect(onAddTempHpClick).not.toHaveBeenCalledWith(NaN);
  });
});