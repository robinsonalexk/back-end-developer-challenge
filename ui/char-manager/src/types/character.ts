import type { DamageTypes } from "./damageTypes";
import type { DefenseTypes } from "./defenseTypes";

export interface Character {
    name: string;
    level: number;
    hitPoints: number;
    currentHitPoints: number;
    tempHitPoints: number;
    stats: Stats;
    classes: Class[];
    items: Item[];
    defenses: Defense[];
}

export interface Stats {
    strength: number;
    dexterity: number;
    constitution: number;
    intelligence: number;
    wisdom: number;
    charisma: number;
}

export interface Class {
    name: string;
    hitDiceValue: number;
    classLevel: number;
}

export interface Item {
    name: string;
    modifier: Modifier
}

export interface Modifier {
    affectedObject: string;
    affectedValue: string;
    value: number;
}

export interface Defense {
    type: DamageTypes;
    defense: DefenseTypes;
}

export const blankCharacter = {
    name: "",
    level: 0,
    hitPoints: 0,
    currentHitPoints: 0,
    tempHitPoints: 0,
    stats: {
        strength: 0,
        dexterity: 0,
        constitution: 0,
        intelligence: 0,
        wisdom: 0,
        charisma: 0,
    },
    classes: [],
    items: [],
    defenses: []
};