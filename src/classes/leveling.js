import { LEVEL_UP_EMOJIS, LEVEL_SHOW_EMOJIS, IsAdminOrOwner, RequiredLevelXp } from '../config.js';
import fs from 'node:fs';
import path from 'node:path';

class Leveling {
    constructor(client) {
        this.client = client;
        this.xpData = this.LoadXpData();
        this.config = this.LoadConfig();
        this.lastMessageTime = {};

        this.client.leveling = this;
        this.client.on('messageCreate', this.OnMessage.bind(this));
    }

    LoadXpData() {
        try {
            const data = fs.readFileSync(path.join(__dirname, 'xp_data.json'), 'utf8');
            return JSON.parse(data);
        } catch (err) {
            if (err.code === 'ENOENT') {
                return {};
            // biome-ignore lint/style/noUselessElse: <explanation>
            } else {
                console.error(err);
                return {};
            }
        }
    }

    SaveXp(data) {
        fs.writeFileSync(path.join(__dirname, 'xp_data.json'), JSON.stringify(data, null, 4));
    }

    LoadConfig() {
        try {
            const data = fs.readFileSync(path.join(__dirname, 'config.json'), 'utf8');
            return JSON.parse(data);
        } catch (err) {
            if (err.code === 'ENOENT') {
                return {};
            // biome-ignore lint/style/noUselessElse: <explanation>
            } else {
                console.error(err);
                return {};
            }
        }
    }

    SaveConfig(data) {
        fs.writeFileSync(path.join(__dirname, 'config.json'), JSON.stringify(data, null, 4));
    }

    async OnMessage(message) {
        if (message.author.bot) return;
        if (message.channel.type === 'DM') return;

        const userId = message.author.id;
        const guildId = message.guild.id;

        const currentTime = Date.now();
        if (this.lastMessageTime[userId] && currentTime - this.lastMessageTime[userId] < 10000) return;

        this.lastMessageTime[userId] = currentTime;

        if (!this.xpData[guildId]) {
            this.xpData[guildId] = {};
        }

        if (!this.xpData[guildId][userId]) {
            this.xpData[guildId][userId] = {
                xp: 0,
                level: 0
            };
        }

        const xpGain = Math.floor(Math.random() * 5) + 4;
        this.xpData[guildId][userId].xp += xpGain;

        const level = this.xpData[guildId][userId].level;
        const xpNeeded = RequiredLevelXp(level);

        const randomEmoji = LEVEL_UP_EMOJIS[Math.floor(Math.random() * LEVEL_UP_EMOJIS.length)];

        if (this.xpData[guildId][userId].xp >= xpNeeded) {
            const oldLevel = this.xpData[guildId][userId].level;
            this.xpData[guildId][userId].level += 1;
            this.xpData[guildId][userId].xp = 0;

            if (this.xpData[guildId][userId].level > oldLevel) {
                const channelId = this.config[guildId]?.levelup_channel;
                const levelupChannel = channelId ? this.client.channels.cache.get(channelId) : message.channel;

                await levelupChannel.send(`${randomEmoji} Congratulations ${message.author}, you have leveled up to level ${this.xpData[guildId][userId].level}!`);
            }
        }

        this.SaveXp(this.xpData);
    }

    async levelCommand(message) {
        const userId = message.author.id;
        const guildId = message.guild.id;

        const randomEmoji = LEVEL_SHOW_EMOJIS[Math.floor(Math.random() * LEVEL_SHOW_EMOJIS.length)];

        // biome-ignore lint/complexity/useOptionalChain: <explanation>
        if (this.xpData[guildId] && this.xpData[guildId][userId]) {
            const level = this.xpData[guildId][userId].level;
            const xp = this.xpData[guildId][userId].xp;
            const xpNeeded = RequiredLevelXp(level);

            await message.reply(`${randomEmoji} ${message.author}, you are level **${level}** with **${xp}/${xpNeeded} XP**.`);
        } else {
            await message.reply(`🤔 ${message.author}, you haven't earned any XP yet!`);
        }
    }

    async SetLevelupChannelCommand(interaction) {
        const guildId = interaction.guild.id;
        const channel = interaction.options.getChannel('channel');

        if (!IsAdminOrOwner(interaction.member)) {
            await interaction.reply('You do not have permission to set the level up channel.');
            return;
        }

        if (!this.config[guildId]) {
            this.config[guildId] = {};
        }

        const currentChannelId = this.config[guildId].levelup_channel;
        const newChannelId = channel.id;

        if (currentChannelId === newChannelId) {
            await interaction.reply(`Level up messages are already set to ${channel}.`);
            return;
        }

        this.config[guildId].levelup_channel = newChannelId;
        this.SaveConfig(this.config);

        await interaction.reply(`Level up messages will now be sent in ${channel}.`);
    }
}

export default Leveling;