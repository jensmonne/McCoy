import { IsAdminOrOwner } from '../config.js';

export const data = {
    name: 'setlevelupchannel',
    description: 'Sets the channel for level up messages.',
    options: [
        {
            name: 'channel',
            type: 'CHANNEL',
            description: 'The channel to send level up messages in',
            required: true,
        },
    ],
    aliases: ['setlevelchannel', 'setlvlchannel', 'slc'],
};

export async function execute(interaction) {
    const leveling = interaction.client.leveling;
    await leveling.SetLevelupChannelCommand(interaction);
}