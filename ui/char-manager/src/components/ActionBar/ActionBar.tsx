import { useState } from "react";
import { DamageTypes } from "../../types/damageTypes";
import Card from "../Card";
import classes from "./ActionBar.module.css"

interface Props {
    onDamageClick: (amount: number, damageType: DamageTypes) => void;
    onHealClick: (amount: number) => void;
    onAddTempHpClick: (amount: number) => void;
}

export default function ActionBar(props: Props) {
    const [damageAmount, setDamageAmount] = useState<string>("");
    const [damageType, setDamageType] = useState<DamageTypes>(DamageTypes.Bludgeoning);
    const [healAmount, setHealAmount] = useState<string>("");
    const [tempHpAmount, setTempHpAmount] = useState<string>("");

    return (
        <Card>
            <div className={classes.inputGroup}>
                <div className={classes.inputRow}>
                    <label>Enter Damage Amount:
                        <input
                            type="text"
                            inputMode="numeric"
                            value={damageAmount}
                            onChange={(e) => setDamageAmount(e.target.value.replace(/\D/g, ""))}
                        />
                    </label>
                    <label>Select Damage Type
                        <select value={damageType} onChange={(e) => setDamageType(e.target.value as DamageTypes)}>
                            {Object.values(DamageTypes).map((dt) => {
                                return (
                                    <option key={dt} value={dt}>{dt}</option>
                                )
                            })}
                        </select>
                    </label>
                    <button onClick={() => props.onDamageClick(Number(damageAmount), damageType)}>Deal Damage</button>
                </div>
                <div className={classes.inputRow}>
                    <label>Enter Healing Amount:
                        <input
                            type="text"
                            inputMode="numeric"
                            value={healAmount}
                            onChange={(e) => setHealAmount(e.target.value.replace(/\D/g, ""))}
                        />
                    </label>
                    <button onClick={() => props.onHealClick(Number(healAmount))}>Heal</button>
                </div>
                <div className={classes.inputRow}>
                    <label>Enter Temporary HP Amount:
                        <input
                            type="text"
                            inputMode="numeric"
                            value={tempHpAmount}
                            onChange={(e) => setTempHpAmount(e.target.value.replace(/\D/g, ""))}
                        />
                    </label>
                    <button onClick={() => props.onAddTempHpClick(Number(tempHpAmount))}>Set Temporary HP</button>
                </div>
            </div>
        </Card>
    )
}