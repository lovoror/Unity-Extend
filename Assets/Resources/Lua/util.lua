local setmetatable, getmetatable, rawset, rawget, pairs, type, ipairs = setmetatable, getmetatable, rawset, rawget, pairs, type, ipairs
local insert = table.insert
local find, format, sub, gsub = string.find, string.format, string.sub, string.gsub
local tonumber, tostring = tonumber, tostring

local function parse_path(path)
    if not path or path == '' then error('invalid path:' .. tostring(path)) end
    --print('start to parse ' .. path)
    local result = {}
    local i, n = 1, #path
    while i <= n do
        local s, e, split1, key, split2 = find(path, "([%.%[])([^%.^%[^%]]+)(%]?)", i)
        if not s or s > i then
            --print('"'.. sub(path, i, s and s - 1).. '"')
            insert(result, sub(path, i, s and s - 1))
        end
        if not s then break end
        if split1 == '[' then
            if split2 ~= ']' then error('invalid path:' .. path) end
            key = tonumber(key)
            if not key then error('invalid path:' .. path) end
            --print(key)
            insert(result, key)
        else
            --print('"'.. key .. '"')
            insert(result, key)
        end
        i = e + 1
    end
    --print('finish parse ' .. path)
    return result
end

local function gen_callback_func(tbl, path, callback)
    local keys = parse_path(path)
    local final
    local finalKey
    for index, key in ipairs(keys) do
        if #keys == index then
            finalKey = key
            break
        end
        final = tbl[key]
    end
    final.__notifications = final.__notifications or {}
    local notifications = final.__notifications[finalKey] or {}
    insert(notifications, callback)
    final.__notifications[finalKey] = notifications
end

return {
    parse_path = parse_path,
    gen_callback_func = gen_callback_func
}