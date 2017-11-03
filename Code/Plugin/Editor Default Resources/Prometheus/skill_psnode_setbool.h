#pragma once

#include "node.hpp"
#include "skill.h"

namespace pvp {

class PhaseNode_SetBool : public SkillPhaseNode {
public:
    enum EType_Trigger {
        TRIGGER_DONE = 0,
        TRIGGER_MAX = 1,
    };
    enum EType_Func {
        FUNC_SET = 0,
    };
public:
    PhaseNode_SetBool(uint16_t id, SkillPhase* phase);
    virtual ~PhaseNode_SetBool();

    //节点更新
    virtual void update(SkillRunFlow* flow, uint32_t ms);

    //序列化
    virtual void serializeNode(const PlatoPhaseNodeSync* sync);

    //反序列化
    virtual void deSerializeNode(PlatoPhaseNodeSync* sync);

    DEFINE_INTERFACE(Set);

    GETTER_SETTER_PLATO(Name, std::string, name, 1);
    GETTER_SETTER_PLATO(Value, bool, value, 2);
private:
    CLASS_FIELD(name, std::string);
    CLASS_FIELD(value, bool);
};

} // namespace pvp
