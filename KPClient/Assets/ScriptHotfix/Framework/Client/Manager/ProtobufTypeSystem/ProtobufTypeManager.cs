using System;
using System.Collections.Generic;
using System.Reflection;
using NexCore.DI;
using BoysheO.Extensions;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace HotScripts.Hotfix.GamePlay.Helper
{
    [Service(typeof(ProtobufTypeManager))]
    public class ProtobufTypeManager
    {
        private Dictionary<Type, MessageDescriptor> _pbType2Desc = new();

        public IMessage Parse(byte[] bytes, Type type)
        {
            ThrowIfNotMessageType(type);
            var parser = GetMessageParser(type);
            return parser.ParseFrom(bytes);
        }
        
        public T Parse<T>(byte[] bytes) where T : IMessage<T>
        {
            var parser = GetMessageParser(typeof(T)) as MessageParser<T> ?? throw new Exception("get parser fail");
            return parser.ParseFrom(bytes);
        }

        public MessageParser GetMessageParser(Type type)
        {
            ThrowIfNotMessageType(type);
            var desc = GetMessageDescriptor(type);
            return desc.Parser;
        }

        public MessageDescriptor GetMessageDescriptor(Type type)
        {
            ThrowIfNotMessageType(type);
            if (!_pbType2Desc.TryGetValue(type, out var desc))
            {
                var property = type.GetProperty(nameof(MessageDescriptor), BindingFlags.Public | BindingFlags.Static) ??
                               throw new ArgumentNullException(nameof(type));
                desc = property.GetValue(null) as MessageDescriptor ?? throw new Exception("read value fail");
                _pbType2Desc[type] = desc;
            }

            return desc;
        }

        private void ThrowIfNotMessageType(Type type)
        {
            if (!type.IsClassAndImplement(typeof(Google.Protobuf.IMessage)))
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}