#pragma once

#include "node.hpp"
#include "skill.h"

namespace pvp {

class PhaseNode_ConfirmPhase : public SkillPhaseNode {
public:
    enum EType_Trigger {
        TRIGGER_CONFIRM = 0,
        TRIGGER_MAX = 1,
    };
public:
    PhaseNode_ConfirmPhase(uint16_t id, SkillPhase* phase);
    virtual ~PhaseNode_ConfirmPhase();

    //节点更新
    virtual void update(SkillRunFlow* flow, uint32_t ms);

    //序列化
    virtual void serializeNode(const PlatoPhaseNodeSync* sync);

    //反序列化
    virtual void deSerializeNode(PlatoPhaseNodeSync* sync);

    //start接口
    virtual bool start(SkillRunFlow* flow);

private:
};

} // namespace pvp
