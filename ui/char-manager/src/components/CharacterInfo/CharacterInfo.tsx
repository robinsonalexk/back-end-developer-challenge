import type { Character, Class, Defense } from "../../types/character";
import { capitalizeWord } from "../../utils/strUtils";
import Card from "../Card";
import classes from "./CharacterInfo.module.css"

interface Props {
    character?: Character
}

export default function CharacterInfo(props: Props) {
    const { character } = props;

    return (
        <Card>
            {character ?
                (<div className={classes.group}>
                    <section className={classes.section}>
                        <h4>General</h4>
                        <ul className={classes.list}>
                            <li>Name: {character?.name}</li>
                            <li>Level: {character?.level}</li>
                            <li aria-label={`Hit Points: ${character?.currentHitPoints} out of ${character?.hitPoints}`}>Hit Points: {character?.currentHitPoints}/{character?.hitPoints}</li>
                            <li>Temporary Hit Points: {character?.tempHitPoints}</li>
                        </ul>
                    </section>
                    <section className={classes.section}>
                        <h4>Class Info</h4>
                        <ul className={classes.list}>
                            {character && character.classes.map((c: Class) => {
                                return (
                                    <li key={`${c.name}-level-${character.name}`}>{capitalizeWord(c.name)}: Level {c.classLevel}</li>
                                )
                            })}
                        </ul>
                    </section>
                    <section className={classes.section}>
                        <h4>Ability Scores</h4>
                        <ul className={classes.list}>
                            <li>Strength: {character?.stats.strength}</li>
                            <li>Dexterity: {character?.stats.dexterity}</li>
                            <li>Constitution: {character?.stats.constitution}</li>
                            <li>Intelligence: {character?.stats.intelligence}</li>
                            <li>Wisdom: {character?.stats.wisdom}</li>
                            <li>Charisma: {character?.stats.charisma}</li>
                        </ul>
                    </section>
                    <section className={classes.section}>
                        <h4>Defenses</h4>
                        <ul className={classes.list}>
                            {character.defenses.map((c: Defense) => {
                                return (
                                    <li key={`${c.type}-${character.name}`}>
                                        {c.type}: {c.defense}
                                    </li>
                                )
                            }
                            )}
                        </ul>
                    </section>
                </div>) : <> No Character Selected </>
            }
        </Card>
    )
}