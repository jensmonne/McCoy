import { LEVEL_SHOW_EMOJIS, RequiredLevelXp } from '../config.js';

export const data = {
    name: 'level',
    description: 'Shows your current level and XP.',
    aliases: ['lvl', 'xp'],
};

export async function execute(message, args) {
    const leveling = message.client.leveling;
    await leveling.levelCommand(message);
}