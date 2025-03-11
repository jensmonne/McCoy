import { Events } from 'discord.js';

export const name = Events.ClientReady;
export const once = true;

export function execute(readyClient) {
    console.log(`Logged in as ${readyClient.user.tag}`);
}