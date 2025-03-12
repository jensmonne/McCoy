import { Client, Collection, GatewayIntentBits } from 'discord.js';
import fs from 'node:fs';
import path from 'node:path';
import Leveling from './commands/leveling.js';
import dotenv from 'dotenv';

dotenv.config();

// Get the token from the .env file and set the prefix
const token = process.env.DISCORD_TOKEN;
const prefix = '$';

const client = new Client({ intents: [GatewayIntentBits.Guilds, GatewayIntentBits.GuildMessages, GatewayIntentBits.MessageContent] });
client.commands = new Collection();

const commandPath = path.join(__dirname, 'commands');
const commandFiles = fs.readdirSync(commandPath).filter(file => file.endsWith('.js'));

await Promise.all(commandFiles.map(async (file) => {
    const command = await import(path.join(commandPath, file));
    client.commands.set(command.data.name, command);
}));

const eventPath = path.join(__dirname, 'events');
const eventFiles = fs.readdirSync(eventPath).filter(file => file.endsWith('.js'));

await Promise.all(eventFiles.map(async (file) => {
    const event = await import(path.join(eventPath, file));
    if (event.once) {
        client.once(event.name, (...args) => event.execute(...args, client));
    } else {
        client.on(event.name, (...args) => event.execute(...args, client));
    }
}));

// Initialize leveling system
const leveling = new Leveling(client);

client.on('messageCreate', async message => {
    if (!message.content.startsWith(prefix) || message.author.bot) return;

    const args = message.content.slice(prefix.length).trim().split(/ +/);
    const commandName = args.shift().toLowerCase();

    const command = client.commands.get(commandName);

    if (!command) return;

    try {
        await command.execute(message, args);
    } catch (error) {
        console.error(error);
        await message.reply({ content: 'There was an error while executing this command!', ephemeral: true });
    }
});

// Run the bot
client.login(token);